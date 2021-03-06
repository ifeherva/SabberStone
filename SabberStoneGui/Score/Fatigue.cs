namespace SabberStoneCoreGui.Score
{
    public class FatigueScore : Score
    {
        public override int Rate()
        {
            if (OpHeroHp < 1)
                return int.MaxValue;

            if (HeroHp < 1)
                return int.MinValue;

            var result = 0;

            if (OpBoard.Count == 0 && Board.Count > 0)
                result += 1000;

            if (OpHandCnt > 9)
                result += 1000;

            if (OpDeckCnt == 0)
                result += 10000;

            result += (DeckCnt - OpDeckCnt) * 50;

            result += (Board.Count - OpBoard.Count) * 10;

            result += (MinionTotHealthTaunt - OpMinionTotHealthTaunt) * 10;

            result += MinionTotAtk;

            result += (HeroHp - OpHeroHp) * 5;

            return result;
        }
    }
}