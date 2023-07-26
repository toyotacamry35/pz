using Assets.Src.Aspects.Impl.Factions.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace Assets.ColonyShared.GeneratedCode.Manual.QuestStaff
{
    public class QuestDiagnosticInfo
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const double _threshold = 500.0;

        private DateTime _startedAt;
        private DateTime _endedAt;
        private bool _complete;
        private bool _fastCompleted;

        public event Action<QuestDiagnosticInfo> OnComplete;
        public event Action<QuestDiagnosticInfo> OnFastComplete;

        public QuestDef Quest { get; private set; }
        public bool FastCompleted {
            get => _fastCompleted;
            private set
            {
                _fastCompleted = value;
                if(_fastCompleted)
                    OnFastComplete?.Invoke(this);
            }
        }

        public DateTime StartedAt
        {
            get => _startedAt;
            set
            {
                if (!Check(value, _startedAt, nameof(StartedAt)))
                    return;

                _startedAt = value;
            }
        }
        public DateTime EndedAt
        {
            get => _endedAt;
            set
            {
                if (!Check(value, _endedAt, nameof(EndedAt)))
                    return;

                _endedAt = value;
                Elapsed = _endedAt - _startedAt;
                Completed = true;
            }
        }
        public TimeSpan Elapsed { get; private set; }

        public bool Completed { get => _complete;
            private set
            {
                _complete = value;
                OnComplete?.Invoke(this);
                if (Elapsed.TotalMilliseconds <= _threshold)
                    FastCompleted = true;
            }

        }

        public QuestDiagnosticInfo(QuestDef quest)
        {
            Quest = quest;
        }

        private bool Check(DateTime newValue, DateTime propValue, string propName)
        {
            if (Completed)
            {
                Logger.IfWarn()?.Message($"Attempting to set a [{propName}] when Quest is already completed!\n{ToString()}").Write();
                return false;
            }

            if (!propValue.Equals(default))
            {
                Logger.IfWarn()?.Message($"Attempting to set a [{propName}] that is already have value!").Write();
                return false; ;
            }
            if (newValue.Equals(default))
            {
                Logger.IfWarn()?.Message($"Attempting to set[Default value] in [{propName}]").Write();
                return false; ;
            }
            return true;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\n----------[QuestDiagnosticInfo]----------");
            sb.Append("Quest:\t\t").AppendLine(Quest?.ToString()??"NULL");
            sb.Append("StartedAt:\t\t").Append(StartedAt).AppendLine();
            sb.Append("EndedAt:\t\t").Append(EndedAt).AppendLine();
            sb.Append("Elapsed:\t\t").Append(Elapsed).AppendLine();
            sb.Append("Completed:\t").Append(Completed).AppendLine();
            sb.Append("FastComleted:\t").Append(FastCompleted).AppendLine();
            sb.AppendLine("----------[QuestDiagnosticInfo]----------");
            return sb.ToString();
        }
    }
}
