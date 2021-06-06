using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtifactDeckCodeDotNet
{
    public static class ArtifactDeckEncoder
    {
        public static uint CurrentVersion = 2;
        private static string EncodedPrefix = "ADC";
        private static int HeaderSize = 3;

        //expects Deck with Heroes, Cards, and Name
        //	signature cards for heroes SHOULD NOT be included in "cards"
        public static string EncodeDeck(Deck deckContents)
        {
            if (deckContents == null)
                throw new ArgumentNullException(nameof(deckContents));

            List<byte> bytes = EncodeBytes(deckContents);

            string deckCode = EncodeBytesToString(bytes);
            return deckCode;
        }

        private static List<byte> EncodeBytes(Deck deckContents)
        {
            if (deckContents.Heroes == null || deckContents.Cards == null)
                throw new ArgumentNullException();

            deckContents.Heroes = deckContents.Heroes.OrderBy(x => x.Id).ToList();
            deckContents.Cards = deckContents.Cards.OrderBy(x => x.Id).ToList();

            int countHeroes = deckContents.Heroes.Count;
            List<ICard> allCards = deckContents.Heroes.Concat<ICard>(deckContents.Cards).ToList();

            List<byte> bytes = new List<byte>();
            //our version and hero count
            uint version = CurrentVersion << 4 | ExtractNBitsWithCarry(countHeroes, 3);
            AddByte(bytes, version);

            //the checksum which will be updated at the end
            uint dummyChecksum = 0;
            int checksumByte = bytes.Count;
            AddByte(bytes, dummyChecksum);

            // write the name size
            uint nameLen = 0;
            string name = "";
            if (deckContents.Name != null)
            {
                // replace strip_tags() with your own HTML santizer or escaper.
                //name = strip_tags( deckContents['name']);
                name = deckContents.Name;
                int trimLen = name.Length;
                while (trimLen > 63)
                {
                    int amountToTrim = (int)Math.Floor((decimal)(trimLen - 63) / 4);
                    amountToTrim = (amountToTrim > 1) ? amountToTrim : 1;
                    name = name.Substring(0, name.Length - amountToTrim);
                    trimLen = name.Length;
                }

                nameLen = (uint)name.Length;
            }

            AddByte(bytes, nameLen);

            AddRemainingNumberToBuffer(countHeroes, 3, bytes);

            int prevCardId = 0;
            for (int currHero = 0; currHero < countHeroes; currHero++)
            {
                HeroRef card = (HeroRef)allCards[currHero];
                if (card.Turn == 0)
                    throw new Exception("A hero's turn cannot be 0");

                AddCardToBuffer((uint)card.Turn, card.Id - prevCardId, bytes);

                prevCardId = card.Id;
            }

            //reset our card offset
            prevCardId = 0;

            //now all of the cards
            for (int currCard = countHeroes; currCard < allCards.Count; currCard++)
            {
                //see how many cards we can group together
                CardRef card = (CardRef)allCards[currCard];
                if (card.Count == 0)
                    throw new Exception("A card's count cannot be 0");
                if (card.Id <= 0)
                    throw new Exception("A card's id cannot be 0 or less");

                //record this set of cards, and advance
                AddCardToBuffer((uint)card.Count, card.Id - prevCardId, bytes);

                prevCardId = card.Id;
            }

            // save off the pre string bytes for the checksum
            int preStringByteCount = bytes.Count;

            //write the string
            {
                byte[] nameBytes = Encoding.UTF8.GetBytes(name);
                foreach (byte nameByte in nameBytes)
                {
                    AddByte(bytes, nameByte);
                }
            }

            uint unFullChecksum = ComputeChecksum(bytes, preStringByteCount - HeaderSize);
            uint unSmallChecksum = (unFullChecksum & 0x0FF);

            bytes[checksumByte] = Convert.ToByte(unSmallChecksum);
            return bytes;
        }

        private static string EncodeBytesToString(List<byte> bytes)
        {
            int byteCount = bytes.Count;
            //if we have an empty buffer, just throw
            if (byteCount == 0)
                throw new Exception("No deck content");

            string encoded = Convert.ToBase64String(bytes.ToArray());
            string deckString = EncodedPrefix + encoded;

            deckString = deckString.Replace('/', '-');
            deckString = deckString.Replace('=', '_');

            return deckString;
        }

        private static uint ExtractNBitsWithCarry(int value, int numBits)
        {
            uint limitBit = (uint)1 << numBits;
            uint result = (uint)(value & (limitBit - 1));
            if (value >= limitBit)
            {
                result |= limitBit;
            }

            return result;
        }

        private static void AddByte(List<byte> bytes, uint b)
        {
            if (b > 255)
                throw new Exception("Invalid byte value");

            bytes.Add(Convert.ToByte(b));
        }

        //utility to write the rest of a number into a buffer. This will first strip the specified N bits off, and then write a series of bytes of the structure of 1 overflow bit and 7 data bits
        private static void AddRemainingNumberToBuffer(int value, int alreadyWrittenBits, List<byte> bytes)
        {
            value >>= alreadyWrittenBits;
            int numBytes = 0;
            while (value > 0)
            {
                uint nextByte = ExtractNBitsWithCarry(value, 7);
                value >>= 7;
                AddByte(bytes, nextByte);

                numBytes++;
            }
        }

        private static void AddCardToBuffer(uint count, int value, List<byte> bytes)
        {
            //this shouldn't ever be the case
            if (count == 0)
                throw new Exception($"{count} is 0, this shouldn't ever be the case");

            int countBytesStart = bytes.Count;

            //determine our count. We can only store 2 bits, and we know the value is at least one, so we can encode values 1-5. However, we set both bits to indicate an 
            //extended count encoding
            uint firstByteMaxCount = 0x03;
            bool extendedCount = (count - 1) >= firstByteMaxCount;

            //determine our first byte, which contains our count, a continue flag, and the first few bits of our value
            uint firstByteCount = extendedCount ? firstByteMaxCount : /*( uint8 )*/(count - 1);
            uint firstByte = (firstByteCount << 6);
            firstByte |= ExtractNBitsWithCarry(value, 5);

            AddByte(bytes, firstByte);

            //now continue writing out the rest of the number with a carry flag
            AddRemainingNumberToBuffer(value, 5, bytes);

            //now if we overflowed on the count, encode the remaining count
            if (extendedCount)
            {
                AddRemainingNumberToBuffer((int)count, 0, bytes);
            }

            int countBytesEnd = bytes.Count;

            if (countBytesEnd - countBytesStart > 11)
            {
                //something went horribly wrong
                throw new Exception($"{nameof(countBytesEnd)} - {nameof(countBytesStart)} is more than 11, something went horribly wrong");
            }
        }

        private static uint ComputeChecksum(List<byte> bytes, int numBytes)
        {
            uint checksum = 0;
            for (int addCheck = HeaderSize; addCheck < numBytes + HeaderSize; addCheck++)
            {
                byte b = bytes[addCheck];
                checksum += b;
            }

            return checksum;
        }
    }
}
