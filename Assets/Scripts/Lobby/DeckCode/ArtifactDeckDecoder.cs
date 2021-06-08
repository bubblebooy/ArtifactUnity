using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtifactDeckCodeDotNet
{
    public static class ArtifactDeckDecoder
    {
        public static int CurrentVersion = 2;
        private static string EncodedPrefix = "RTFACT";

        //returns Deck with Heroes, Cards, and Name
        public static Deck ParseDeck(string deckCode)
        {
            byte[] deckBytes = DecodeDeckString(deckCode);

            Deck deck = ParseDeckInternal(deckCode, deckBytes);
            return deck;
        }

        public static byte[] RawDeckBytes(string deckCode)
        {
            byte[] deckBytes = DecodeDeckString(deckCode);
            return deckBytes;
        }

        private static byte[] DecodeDeckString(string deckCode)
        {
            //check for prefix
            
            if (deckCode.Substring(0, EncodedPrefix.Length) != EncodedPrefix)
                throw new Exception("Artifact Deck Code prefix missing");

            //strip prefix from deck code
            deckCode = deckCode.Substring(EncodedPrefix.Length);

            // deck strings are base64 but with url compatible strings, put the URL special chars back
            deckCode = deckCode.Replace('-', '/');
            deckCode = deckCode.Replace('_', '=');
            byte[] decoded = Convert.FromBase64String(deckCode);
            return decoded;
        }

        //reads out a var-int encoded block of bits, returns true if another chunk should follow
        private static bool ReadBitsChunk(int chunk, int numBits, int currShift, ref int outBits)
        {
            int continueBit = (1 << numBits);
            int newBits = chunk & (continueBit - 1);
            outBits |= (newBits << currShift);

            return (chunk & continueBit) != 0;
        }

        private static bool ReadVarEncodedUint32(int baseValue, int baseBits, byte[] data, ref int indexStart, int indexEnd, ref int outValue)
        {
            outValue = 0;

            int deltaShift = 0;
            if ((baseBits == 0) || ReadBitsChunk(baseValue, baseBits, deltaShift, ref outValue))
            {
                deltaShift += baseBits;

                while (true)
                {
                    //do we have more room?
                    if (indexStart > indexEnd)
                        return false;

                    //read the bits from this next byte and see if we are done
                    int nextByte = data[indexStart++];
                    if (!ReadBitsChunk(nextByte, 7, deltaShift, ref outValue))
                        break;

                    deltaShift += 7;
                }
            }

            return true;
        }

        //handles decoding a card that was serialized
        private static bool ReadSerializedCard(byte[] data, ref int indexStart, int indexEnd, ref int prevCardBase, ref int outCount, ref int outCardId)
        {
            //end of the memory block?
            if (indexStart > indexEnd)
                return false;

            //header contains the count (2 bits), a continue flag, and 5 bits of offset data. If we have 11 for the count bits we have the count
            //encoded after the offset
            byte header = data[indexStart++];
            bool hasExtendedCount = ((header >> 6) == 0x03);

            //read in the delta, which has 5 bits in the header, then additional bytes while the value is set
            int cardDelta = 0;
            if (!ReadVarEncodedUint32(header, 5, data, ref indexStart, indexEnd, ref cardDelta))
                return false;

            outCardId = prevCardBase + cardDelta;

            //now parse the count if we have an extended count
            if (hasExtendedCount)
            {
                if (!ReadVarEncodedUint32(0, 0, data, ref indexStart, indexEnd, ref outCount))
                    return false;
            }
            else
            {
                //the count is just the upper two bits + 1 (since we don't encode zero)
                outCount = (header >> 6) + 1;
            }

            //update our previous card before we do the remap, since it was encoded without the remap
            prevCardBase = outCardId;
            return true;
        }

        private static Deck ParseDeckInternal(string deckCode, byte[] deckBytes)
        {
            int currentByteIndex = 0; // 0 instead of 1, deckBytes starts at 1 in PHP
            int totalBytes = deckBytes.Length;

            //check version num
            byte versionAndHeroes = deckBytes[currentByteIndex++];
            int version = versionAndHeroes >> 4;
            if (CurrentVersion != version && version != 1)
                throw new Exception("Invalid code version");

            //do checksum check
            byte checksum = deckBytes[currentByteIndex++];

            int stringLength = 0;
            if (version > 1)
                stringLength = deckBytes[currentByteIndex++];

            int totalCardBytes = totalBytes - stringLength;

            //grab the string size
            {
                int computedChecksum = 0;
                for (int i = currentByteIndex; i < totalCardBytes; i++)
                    computedChecksum += deckBytes[i];

                int masked = (computedChecksum & 0xFF);
                if (checksum != masked)
                    throw new Exception("checksum does not match");
            }

            //read in our hero count (part of the bits are in the version, but we can overflow bits here
            int numHeroes = 0;
            if (!ReadVarEncodedUint32(versionAndHeroes, 3, deckBytes, ref currentByteIndex, totalCardBytes, ref numHeroes))
                throw new Exception("Missing hero count");

            //now read in the heroes
            List<HeroRef> heroes = new List<HeroRef>();
            int prevCardBase = 0;
            {
                for (int currHero = 0; currHero < numHeroes; currHero++)
                {
                    int heroTurn = 0;
                    int heroCardId = 0;
                    if (!ReadSerializedCard(deckBytes, ref currentByteIndex, totalCardBytes, ref prevCardBase, ref heroTurn, ref heroCardId))
                    {
                        throw new Exception("Missing hero data");
                    }

                    heroes.Add(new HeroRef { Id = heroCardId, Turn = heroTurn });
                }
            }

            List<CardRef> cards = new List<CardRef>();
            prevCardBase = 0;
            while (currentByteIndex < totalCardBytes) // < instead of <=, deckBytes starts at 1 in PHP
            {
                int cardCount = 0;
                int cardId = 0;
                if (!ReadSerializedCard(deckBytes, ref currentByteIndex, totalBytes, ref prevCardBase, ref cardCount, ref cardId))
                    throw new Exception("Missing card data");

                cards.Add(new CardRef { Id = cardId, Count = cardCount });
            }

            string name = "";
            if (currentByteIndex < totalBytes) // < instead of <=, deckBytes starts at 1 in PHP
            {
                var bytes = deckBytes.Skip(deckBytes.Length - stringLength).ToArray();
                name = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }

            return new Deck { Heroes = heroes, Cards = cards, Name = name };
        }
    }
}
