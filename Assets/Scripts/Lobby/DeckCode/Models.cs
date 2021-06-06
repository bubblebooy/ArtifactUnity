using System.Collections.Generic;

namespace ArtifactDeckCodeDotNet
{
    public interface ICard
    {
        int Id { get; set; }
    }

    public class HeroRef : ICard
    {
        public int Id { get; set; }
        public int Turn { get; set; }
    }

    public class CardRef : ICard
    {
        public int Id { get; set; }
        public int Count { get; set; }
    }

    public class Deck
    {
        public string Name { get; set; }
        public List<HeroRef> Heroes { get; set; }
        public List<CardRef> Cards { get; set; }
    }
}
