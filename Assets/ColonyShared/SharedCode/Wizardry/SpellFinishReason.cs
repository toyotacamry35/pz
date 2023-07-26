using System;

namespace SharedCode.Wizardry
{
    public enum SpellFinishReason : byte
    {
        None,
        SucessOnTime, //spell finished on time
        SucessOnDemand, //spell has been stopped and succeded (say, finished by user request)
        FailOnStart, //spell failed to start
        FailOnDemand, //spell has been stopped and failed (say, has been stopped by some other spell)
        FailOnEnd //spell failed on finish
    }

    public static class SpellFinishReasonExtensions
    {
        public static bool IsFail(this SpellFinishReason reason)
        {
            switch (reason)
            {
                case SpellFinishReason.FailOnStart:
                case SpellFinishReason.FailOnDemand:
                case SpellFinishReason.FailOnEnd:
                    return true;
            }
            return false;
        }
        
        public static bool IsSuccess(this SpellFinishReason reason)
        {
            switch (reason)
            {
                case SpellFinishReason.SucessOnTime:
                case SpellFinishReason.SucessOnDemand:
                    return true;
            }
            return false;
        }
    }
}
