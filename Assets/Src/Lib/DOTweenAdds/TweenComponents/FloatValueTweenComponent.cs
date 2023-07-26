namespace Assets.Src.Lib.DOTweenAdds
{
    class FloatValueTweenComponent : TweenComponentBase
    {
        public float Amount;


        //=== Props ===========================================================

        protected override float Parameter
        {
            get { return Amount; }
            set { Amount = value; }
        }
    }
}