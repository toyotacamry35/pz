using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ResourcesSystem.Loader;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using NLog;
using NUnit.Framework;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using ResourcesSystem;
using ResourceSystem.Aspects;
using ResourceSystem.Aspects.ManualDefsForSpells;

[TestFixture]
public class TimelineTests
{
    private static Dictionary<string, string> Spells => new Dictionary<string, string>
    {
        {"/" + nameof(SpellA), SpellA},
        {"/" + nameof(SpellMustNotFail), SpellMustNotFail},
        {"/" + nameof(SpellMustFail), SpellMustFail},
    };
    

    private SpellDef _spellA;

    private static string SpellA = @"{
    '$type': 'Spell',
    'IsInfinite': true,
    'Words': [ { '$type': 'EffectTest' } ],
    'SubSpells': [
        {
          '$type': 'SubSpell',
          'OffsetStart': 0.1,
          'Periodic': true,
          'Spell': {
            '$type': 'Spell',
            'Duration': 0.5,
            'Words': [ { '$type': 'EffectTest' } ],
            'SubSpells': [
              {
                '$type': 'SubSpell',
                'OffsetStart': 0.15,
                'Spell': {
                  '$type': 'Spell',
                  'Duration': 0.35,
                  'Words': [ { '$type': 'EffectTest' } ],
                }
              }
            ]
          }
        }
    ]
}";
   
    private SpellDef _spellMustNotFail;

    private static string SpellMustNotFail = 
@"{
    '$type': 'Spell',
    'IsInfinite': true,
    'Words': [],
    'SubSpells': [
        {
            '$type': 'SubSpell',
            'MustNotFail': false,
            'Spell': {
                '$type': 'Spell',
                'IsInfinite': true,
                'Words': [ 
                    { 
                        '$type': 'SpellPredicateTrue',                        
                    },
                    { 
                        '$type': 'EffectTest' 
                    } 
                ],
                'SubSpells': [
                    {
                        '$type': 'SubSpell',
                        'MustNotFail': true,
                        'OffsetStart': 1,
                        'Spell': {
                            '$type': 'Spell',
                            'Duration': 0,
                            'Words': [ 
                                { 
                                    '$type': 'SpellPredicateFalse'                        
                                }
                            ]
                        }
                    }
                ]
            }
        },
        {
            '$type': 'SubSpell',
            'MustNotFail': false,
            'Spell': {
                '$type': 'Spell',
                'IsInfinite': true,
                'Words': [ 
                    { 
                        '$type': 'SpellPredicateFalse'
                    } 
                ]
            }
        }
    ]
}";
   
    private SpellDef _spellMustFail;

    private static string SpellMustFail = 
@"{
    '$type': 'Spell',
    'IsInfinite': true,
    'Words': [],
    'SubSpells': [
        {
            '$type': 'SubSpell',
            'MustNotFail': true,
            'Spell': {
                '$type': 'Spell',
                'Duration': 1,
                'Words': [ 
                    { 
                        '$type': 'SpellPredicateFalse'
                    },
                    { 
                        '$type': 'EffectTest' 
                    } 
                ]
            }
        }
    ]
}";
    
    [Test]
    public void TestSubSpells()
    {
        Assert.IsNotNull(_spellA);
        var timeOrigin = SyncTime.Now;
        var spellDef = _spellA; 
        var spell = MockSpell.Create(new SpellId(1), new SpellCast {Def = spellDef, StartAt = timeOrigin});
        int updateId = 0;
        var runner = new TimelineRunner();
        var wizard = new MockWizard(runner,null);
        var actions = wizard.Actions;
        var spellStatus = spell.Status;
        var subSpell1Status = spell.Status.SubSpells.First();
        var subSpell2Status = spell.Status.SubSpells.First().SubSpells.First();
        var spellWord = spellDef.Words[0]; 
        var subSpell1Word = spellDef.SubSpells[0].Spell.Target.Words[0]; 
        var subSpell2Word = spellDef.SubSpells[0].Spell.Target.SubSpells[0].Spell.Target.Words[0]; 
        long nextUpdate = timeOrigin;
        
        runner.PrepareSpell(spell, (x,y) => TimelineHelpers.ExecutionMask.All);

        Logger($"Step #{++updateId} at {nextUpdate.RelTimeToString(timeOrigin)}");
        actions.Clear();
        nextUpdate = runner.UpdateSpell(spell, nextUpdate, wizard).WaitTask().Result;
        Assert.AreEqual(1, actions.Count);
        Assert.AreEqual(timeOrigin + 100, nextUpdate);
        Assert.IsTrue(actions[0].Action == MockWizard.SpellActionType.Start);
        Assert.AreEqual(timeOrigin, actions[0].Data.WordTimeRange.Start);
        Assert.AreEqual(long.MaxValue, actions[0].Data.WordTimeRange.Finish);
        Assert.AreEqual(0, actions[0].Data.SubSpellCount);
        Assert.AreEqual(spellWord, actions[0].Word);
        Assert.AreEqual(1, spellStatus.ActivationsCount);
        Assert.AreEqual(0, spellStatus.DeactivationsCount);

        Logger($"Step #{++updateId} at {(nextUpdate - 1).RelTimeToString(timeOrigin)}");
        actions.Clear();
        nextUpdate = runner.UpdateSpell(spell, nextUpdate - 1, wizard).WaitTask().Result;
        Assert.AreEqual(0, actions.Count);

        Logger($"Step #{++updateId} at {nextUpdate.RelTimeToString(timeOrigin)}");
        actions.Clear();
        nextUpdate = runner.UpdateSpell(spell, nextUpdate, wizard).WaitTask().Result;
        Assert.AreEqual(1, actions.Count);
        Assert.AreEqual(timeOrigin + 100 + 150, nextUpdate);
        Assert.AreEqual(MockWizard.SpellActionType.Start, actions[0].Action);
        Assert.AreEqual(0, actions[0].Data.SubSpellCount);
        Assert.AreEqual(subSpell1Word, actions[0].Word);
        Assert.AreEqual(timeOrigin + 100, actions[0].Data.WordTimeRange.Start);
        Assert.AreEqual(timeOrigin + 600, actions[0].Data.WordTimeRange.Finish);
        Assert.AreEqual(1, subSpell1Status.ActivationsCount);
        Assert.AreEqual(0, subSpell1Status.DeactivationsCount);

        int iter;
        for (iter = 0; iter < 5; ++iter)
        {
            Logger($"Step #{++updateId} at {(nextUpdate - 1).RelTimeToString(timeOrigin)}");
            actions.Clear();
            nextUpdate = runner.UpdateSpell(spell, nextUpdate - 1, wizard).WaitTask().Result;
            Assert.AreEqual(0, actions.Count);

            Logger($"Step #{++updateId} at {nextUpdate.RelTimeToString(timeOrigin)}");
            actions.Clear();
            nextUpdate = runner.UpdateSpell(spell, nextUpdate, wizard).WaitTask().Result;
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(timeOrigin + 100 + 150 + 350 + iter * 500, nextUpdate);
            Assert.AreEqual(MockWizard.SpellActionType.Start, actions[0].Action);
            Assert.AreEqual(0 + iter, actions[0].Data.SubSpellCount);
            Assert.AreEqual(subSpell2Word, actions[0].Word);
            Assert.AreEqual(timeOrigin + 250 + iter * 500, actions[0].Data.WordTimeRange.Start);
            Assert.AreEqual(timeOrigin + 600 + iter * 500, actions[0].Data.WordTimeRange.Finish);
            Assert.AreEqual(1 + iter, subSpell2Status.ActivationsCount);
            Assert.AreEqual(0 + iter, subSpell2Status.DeactivationsCount);

            Logger($"Step #{++updateId} at {(nextUpdate - 1).RelTimeToString(timeOrigin)}");
            actions.Clear();
            nextUpdate = runner.UpdateSpell(spell, nextUpdate - 1, wizard).WaitTask().Result;
            Assert.AreEqual(0, actions.Count);

            Logger($"Step #{++updateId} at {nextUpdate.RelTimeToString(timeOrigin)}");
            actions.Clear();
            nextUpdate = runner.UpdateSpell(spell, nextUpdate, wizard).WaitTask().Result;
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual(timeOrigin + 100 + 150 + 350 + 150 + iter * 500, nextUpdate);
            Assert.AreEqual(MockWizard.SpellActionType.Finish, actions[0].Action);
            Assert.AreEqual(0 + iter, actions[0].Data.SubSpellCount);
            Assert.AreEqual(subSpell2Word, actions[0].Word);
            Assert.AreEqual(MockWizard.SpellActionType.Finish, actions[1].Action);
            Assert.AreEqual(0 + iter, actions[1].Data.SubSpellCount);
            Assert.AreEqual(subSpell1Word, actions[1].Word);
            Assert.AreEqual(MockWizard.SpellActionType.Start, actions[2].Action);
            Assert.AreEqual(1 + iter, actions[2].Data.SubSpellCount);
            Assert.AreEqual(subSpell1Word, actions[2].Word);
            Assert.AreEqual(2 + iter, subSpell1Status.ActivationsCount);
            Assert.AreEqual(1 + iter, subSpell1Status.DeactivationsCount);
            Assert.AreEqual(1 + iter, subSpell2Status.ActivationsCount);
            Assert.AreEqual(1 + iter, subSpell2Status.DeactivationsCount);
        }
        
//        LoggerActive = true;

        spell.Stop(nextUpdate - 225, SpellFinishReason.SucessOnDemand);
        
        Logger($"Step #{++updateId} at {(nextUpdate - 205).RelTimeToString(timeOrigin)} (Final)");
        actions.Clear();
        nextUpdate = runner.UpdateSpell(spell, nextUpdate - 205, wizard).WaitTask().Result;
        Assert.AreEqual(TimelineRunner.RESULT_FINISHED, nextUpdate);
        Assert.AreEqual(2, actions.Count);
        Assert.AreEqual(MockWizard.SpellActionType.Finish, actions[0].Action);
        Assert.AreEqual(0 + iter, actions[0].Data.SubSpellCount);
        Assert.AreEqual(subSpell1Word, actions[0].Word);
        Assert.AreEqual(MockWizard.SpellActionType.Finish, actions[1].Action);
        Assert.AreEqual(0, actions[1].Data.SubSpellCount);
        Assert.AreEqual(spellWord, actions[1].Word);
        Assert.AreEqual(1, spellStatus.ActivationsCount);
        Assert.AreEqual(1, spellStatus.DeactivationsCount);
        Assert.AreEqual(1 + iter, subSpell1Status.ActivationsCount);
        Assert.AreEqual(1 + iter, subSpell1Status.DeactivationsCount);
        Assert.AreEqual(0 + iter, subSpell2Status.ActivationsCount);
        Assert.AreEqual(0 + iter, subSpell2Status.DeactivationsCount);
    }

    [Test]
    public void TestLag()
    {
        Assert.IsNotNull(_spellA);
        var timeOrigin = SyncTime.Now;
        var spellDef = _spellA; 
        var spell = MockSpell.Create(new SpellId(1), new SpellCast {Def = spellDef, StartAt = timeOrigin});
        int updateId = 0;
        var runner = new TimelineRunner();
        var wizard = new MockWizard(runner,null);
        var actions = wizard.Actions;
        var spellStatus = spell.Status;
        var subSpell1Status = spell.Status.SubSpells.First();
        var subSpell2Status = spell.Status.SubSpells.First().SubSpells.First();
        var spellWord = spellDef.Words[0]; 
        var subSpell1Word = spellDef.SubSpells[0].Spell.Target.Words[0]; 
        var subSpell2Word = spellDef.SubSpells[0].Spell.Target.SubSpells[0].Spell.Target.Words[0]; 
        long nextUpdate = timeOrigin;

        runner.PrepareSpell(spell, (x,y) => TimelineHelpers.ExecutionMask.All);

        actions.Clear();
        nextUpdate = runner.UpdateSpell(spell, nextUpdate, wizard).WaitTask().Result;
        Assert.AreEqual(1, actions.Count);
        
        actions.Clear();
        nextUpdate = runner.UpdateSpell(spell, nextUpdate + 10000, wizard).WaitTask().Result;
        Assert.AreEqual(TimelineRunner.MaxActionsPerUpdate, actions.Count);
        Assert.AreEqual(timeOrigin + 100 + 10000 + 10, nextUpdate);

//        LoggerActive = true;
        
        actions.Clear();
        nextUpdate = runner.UpdateSpell(spell, nextUpdate + 10000, wizard).WaitTask().Result;
        Assert.AreEqual(TimelineRunner.MaxActionsPerUpdate, actions.Count);
        Assert.AreEqual(timeOrigin + 100 + 10000 + 10 + 10000 + 10, nextUpdate);
        
        spell.Stop(nextUpdate + 10000, SpellFinishReason.SucessOnDemand);

        actions.Clear();
        nextUpdate = runner.UpdateSpell(spell, nextUpdate + 15000, wizard).WaitTask().Result;
        Assert.AreEqual(TimelineRunner.RESULT_FINISHED, nextUpdate);
        Assert.AreEqual(1, actions.Count);
        Assert.AreEqual(spellStatus.ActivationsCount, spellStatus.DeactivationsCount);
        Assert.AreEqual(subSpell1Status.ActivationsCount, subSpell1Status.DeactivationsCount);
        Assert.AreEqual(subSpell2Status.ActivationsCount, subSpell2Status.DeactivationsCount);
    }

    [Test]
    public void TestMustNotFail()
    {
        Assert.IsNotNull(_spellMustNotFail);
        var timeOrigin = SyncTime.Now;
        var spellDef = _spellMustNotFail;
        var spell = MockSpell.Create(new SpellId(1), new SpellCast {Def = spellDef, StartAt = timeOrigin});
        int updateId = 0;
        var runner = new TimelineRunner();
        var wizard = new MockWizard(runner,null);
        var actions = wizard.Actions;
        var subSpell1Status = spell.Status.SubSpells.First();
        var subSpell2Status = spell.Status.SubSpells.Skip(1).First();
        long nextUpdate = timeOrigin;

        runner.PrepareSpell(spell, (x, y) => TimelineHelpers.ExecutionMask.All);

        Logger($"Step #{++updateId} at {nextUpdate.RelTimeToString(timeOrigin)}");
        actions.Clear();
        nextUpdate = runner.UpdateSpell(spell, nextUpdate, wizard).WaitTask().Result;
        Assert.AreEqual(1, subSpell1Status.SuccesfullPredicatesCheckCount);
        Assert.AreEqual(0, subSpell1Status.FailedPredicatesCheckCount);
        Assert.AreEqual(0, subSpell2Status.SuccesfullPredicatesCheckCount);
        Assert.AreEqual(1, subSpell2Status.FailedPredicatesCheckCount);
        Assert.AreEqual(timeOrigin + 1000, nextUpdate);
        Assert.AreEqual(1, actions.Count);
        Assert.IsFalse(spell.IsFinished);
        nextUpdate = runner.UpdateSpell(spell, nextUpdate, wizard).WaitTask().Result;
        Assert.AreEqual(2, actions.Count);
        Assert.IsTrue(spell.IsFinished);
    }
    
    [Test]
    public void TestMustFail()
    {
        Assert.IsNotNull(_spellMustFail);
        var timeOrigin = SyncTime.Now;
        var spellDef = _spellMustFail;
        var spell = MockSpell.Create(new SpellId(1), new SpellCast {Def = spellDef, StartAt = timeOrigin});
        int updateId = 0;
        var runner = new TimelineRunner();
        var wizard = new MockWizard(runner, null);
        var subSpellStatus = spell.Status.SubSpells.First();
        var actions = wizard.Actions;
        long nextUpdate = timeOrigin;

        runner.PrepareSpell(spell, (x, y) => TimelineHelpers.ExecutionMask.All);

        Logger($"Step #{++updateId} at {nextUpdate.RelTimeToString(timeOrigin)}");
        actions.Clear();
        nextUpdate = runner.UpdateSpell(spell, nextUpdate, wizard).WaitTask().Result;
        Assert.AreEqual(0, subSpellStatus.SuccesfullPredicatesCheckCount);
        Assert.AreEqual(1, subSpellStatus.FailedPredicatesCheckCount);
        Assert.AreEqual(0, actions.Count);
        Assert.IsTrue(spell.IsFinished);
    }

    [TestFixtureSetUp]
    public void Init()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        var gr = new GameResources(new MockLoader(Spells));
        gr.ConfigureForUnityRuntime();
        _spellA = gr.LoadResource<SpellDef>("/" + nameof(SpellA));
        _spellMustNotFail = gr.LoadResource<SpellDef>("/" + nameof(SpellMustNotFail));
        _spellMustFail =  gr.LoadResource<SpellDef>("/" + nameof(SpellMustFail));
    }
    
    private static bool LoggerActive = false;  
    
    private static void Logger(string msg, params object[] args)
    {
        if (LoggerActive)
            TestContext.Out.WriteLine(msg, args);
    }

   #region Mocks 
    private class MockLoader : ILoader
    {
        private readonly Dictionary<string,string> _texts;
        public MockLoader(Dictionary<string,string> texts)
        {
            _texts = texts;
        }
        public IEnumerable<string> AllPossibleRoots { get; }
        public Stream OpenRead(string path) => new MemoryStream(Encoding.UTF8.GetBytes(_texts[path]));
        public Stream OpenWrite(string path) => null;
        public IEnumerable<string> ListAllIn(string path) => Enumerable.Empty<string>();
        public string GetRoot() => string.Empty;
        public bool IsExists(string path) => true;
        public bool IsBinary(string path) => false;
    }

    private class MockSpell : ITimelineSpell
    {
        private int _updating;
        private long _stopCast;
        private SpellFinishReason _stopCastWithReason;
        private SpellFinishReason _finishReason;
        private List<SpellModifierDef> _modifiers;

        public static MockSpell Create(SpellId spellId, ISpellCast castData)
        {
            if (!spellId.IsValid) throw new ArgumentNullException(nameof(spellId));
            if (castData == null) throw new ArgumentNullException(nameof(castData));
            return new MockSpell
            {
                SpellId = spellId,
                CastData = castData,
                Status = MockSpellStatus.Create(castData.Def, null)
            };
        }
        public long StartAt => CastData.StartAt;
        public SpellId SpellId { get; private set; }
        public SpellDef SpellDef => CastData.Def;
        public ISpellCast CastData { get; private set; }
        public IReadOnlyList<SpellModifierDef> Modifiers => _modifiers;
        public ITimelineSpellStatus Status { get; private set; }
        public long StopCast => _stopCast;
        public SpellFinishReason StopCastWithReason => _stopCastWithReason;
        public bool IsAskedToFinish { get; private set; }
        public SpellFinishReason AskedToBeFinishedWithReason { get; private set; }
        public bool IsFinished { get; private set; }
        bool ITimelineSpell.EnterToUpdate()
        {
            if (Interlocked.Increment(ref _updating) > 1)
            {
                if (_updating % 20 == 0)
                    throw new Exception("Can't Enter To Update too many time");
                return false;
            }

            return true;
        }
        bool ITimelineSpell.ExitFromUpdate() => Interlocked.Exchange(ref _updating, 0) > 1;
        public void AskToFinish(long at, SpellFinishReason reason)
        {
            IsAskedToFinish = true;
            AskedToBeFinishedWithReason = reason;
        }
        public void Finish(long at, SpellFinishReason reason)
        {
            IsFinished = true;
            _finishReason = reason;
        }
        public bool Stop(long at, SpellFinishReason reason)
        {
            if (Interlocked.CompareExchange(ref _stopCast, at, 0) == 0)
            {
                _stopCastWithReason = reason;
                return true;
            }

            return false;
        }
        public override string ToString() => ToString(false);
        public string ToString(bool withSubSpells)
        {
            var sb = new StringBuilder();
            sb
                .Append("[")
                .Append("Id:").Append(SpellId)
                .Append(" ").Append(CastData.Def.____GetDebugAddress())
                .Append(" Status:").Append(Status.ToString(false, false));
            if(IsFinished)
                sb.Append(" FIN:").Append(_finishReason);
            else
            if(IsAskedToFinish)
                sb.Append(" ATF:").Append(AskedToBeFinishedWithReason);
            else
            if(StopCast != 0)
                sb.Append(" STP:").Append(StopCastWithReason).Append(":").Append(StopCast);
            if (withSubSpells)
            {
                sb.Append(" SubSpells:[");
                if(Status.SubSpells != null)
                    foreach (var subSpell in Status.SubSpells)
                        sb.Append(subSpell.ToString(true, true));
                sb.Append("] ");
            }
            sb.Append("]");
            return sb.ToString();
        }    
    }


    private class MockSpellStatus : ITimelineSpellStatus
    {
        private List<(SpellWordDef, int)> _wordActivations;
        public int SuccesfullPredicatesCheckCount { get; set; }
        public int FailedPredicatesCheckCount { get; set; }
        public int SuccesfullActivationsCount { get; set; }
        public int ActivationsCount { get; set; }
        public int DeactivationsCount { get; set; }
        public SpellDef SpellDef { get; private set; }
        public SubSpell SubSpell { get; private set; }
        public IEnumerable<ITimelineSpellStatus> SubSpells { get; private set; }
        public SpellTimeLineData TimeLineData { get; set; }
        public IEnumerable<(SpellWordDef, int)> WordActivations => _wordActivations;
        public void IncrementWordActivations(SpellWordDef word, int activationIdx)
        {
            (_wordActivations ?? (_wordActivations = CreateWordActivations(SpellDef))).Add((word, activationIdx));
        }
        public bool DecrementWordActivations(SpellWordDef word)
        {
            if (_wordActivations != null)
                for(int idx = _wordActivations.Count - 1; idx >= 0; --idx)
                    if (_wordActivations[idx].Item1 == word)
                    {
                        _wordActivations.RemoveAt(idx);
                        return true;
                    }
            return false;
        }
        public static MockSpellStatus Create(SpellDef spellDef, SubSpell subSpell)
        {
            return new MockSpellStatus
            {
                SpellDef = spellDef,
                SubSpell = subSpell,
                SubSpells = CreateSubSpells(spellDef.SubSpells),
                //SubSpells = spellDef.SubSpells.Select(x => Create(x.Spell, x)).ToArray(),
            };
        }
        private static MockSpellStatus[] CreateSubSpells(SubSpell[] subSpells)
        {
            var rv = new MockSpellStatus[subSpells.Length];
            for (var i = 0; i < subSpells.Length; i++)
                rv[i] = Create(subSpells[i].Spell, subSpells[i]);
            return rv;
        }
        private static List<(SpellWordDef, int)> CreateWordActivations(SpellDef spellDef)
        {
            var cnt = 0;
            foreach (var word in spellDef.Words)
                if (word is SpellEffectDef)
                    ++cnt;
            return new List<(SpellWordDef, int)>(cnt);
        }
        public override string ToString()
        {
            return ToString(true, false);
        }
        public string ToString(bool withDef, bool withSubSpells)
        {
            var sb = new StringBuilder().Append("[");
            if (withDef)
                sb.Append(SubSpell != null ? SubSpell.____GetDebugAddress() : SpellDef.____GetDebugAddress()).Append(" ");
            sb.Append("A:").Append(ActivationsCount).Append("/").Append(this.FailedActivationsCount());
            sb.Append(" D:").Append(DeactivationsCount);
            if (withSubSpells)
            {
                sb.Append(" SubSpells:[");
                if (SubSpells != null)
                    foreach (var subSpell in SubSpells)
                        sb.Append(subSpell.ToString(true, true));
                sb.Append("] ");
            }
            return sb.Append("]").ToString();
        }
    }


    private class MockWizard : TimelineRunner.IWizard
    {
        private readonly TimelineRunner _timeline;

        public readonly List<SpellAction> Actions = new List<SpellAction>();
        
        public int WordsStarted { get; set; }

        public int WordsFinished { get; set; }

        public int Errors { get; set; }

        IEntitiesRepository _repo;
        public MockWizard(TimelineRunner timeline, IEntitiesRepository repo)
        {
            _timeline = timeline;
            _repo = repo;
        }
        
        bool TimelineRunner.IWizard.NeedToCheckPredicates => true;

        TimelineRunner.LoggerDelegate TimelineRunner.IWizard.Logger(LogLevel level)
        {
            return (m, p) =>
            {
                Logger(m, p);
                if (level >= LogLevel.Error)
                    throw new Exception(string.Format(m, p));
            };
        }

        ValueTask<IEntitiesContainer> TimelineRunner.IWizard.AwaitImportantEntitiesIfNecessary(ISpellCast spellCast)
        {
            return default;
        }
        
        ValueTask<bool> TimelineRunner.IWizard.CheckSpellPredicates(long currentTime, SpellDef spell, ISpellCast castData, SpellId spellId, IReadOnlyList<SpellModifierDef> modifiers)
        {
            bool rv = true;
            foreach (var predicate in spell.Predicates)
            {
                switch (predicate)
                {
                    case SpellPredicateTrueDef _ when !predicate.Inversed:
                    case SpellPredicateFalseDef _ when predicate.Inversed:
                        break;
                    case SpellPredicateTrueDef _ when predicate.Inversed:
                    case SpellPredicateFalseDef _ when !predicate.Inversed:
                        rv = false;
                        break;
                    default:
                        throw new NotSupportedException($"{predicate}");
                }
            }
            return new ValueTask<bool>(rv);
        }

        async Task TimelineRunner.IWizard.SpellFinished(ITimelineSpell spell, long now)
        {
            await _timeline.FinishSpell(spell, now, this);
        }

        ValueTask TimelineRunner.IWizard.StartWord(SpellWordDef word, SpellWordCastData castData)
        {
            if (!(word is EffectTestDef)) throw new NotSupportedException($"{word}");
            WordsStarted++;
            Actions.Add(new SpellAction{ Action = SpellActionType.Start, Data = castData, Word = word});
            return new ValueTask();
        }

        ValueTask TimelineRunner.IWizard.FinishWord(SpellWordDef word, SpellWordCastData castData, bool failed)
        {
            if (!(word is EffectTestDef)) throw new NotSupportedException($"{word}");
            WordsFinished++;
            Actions.Add(new SpellAction{ Action = SpellActionType.Finish, Data = castData, Word = word});
            return new ValueTask();
        }

        SpellWordCastData TimelineRunner.IWizard.CreateWordCastData(long currentTime, long spellStartTime, long parentSubSpellStartTime, TimeRange wordTimeRange, SpellId spellId, int subSpellCount, ISpellCast castData, IReadOnlyList<SpellModifierDef> modifiers)
        {
            return new SpellWordCastData
            (
                wizard: new OuterRef<IWizardEntity>(),
                castData: castData,
                caster: new OuterRef<IEntity>(),
                spellId: spellId,
                subSpellCount: subSpellCount,
                currentTime: currentTime,
                wordTimeRange: wordTimeRange,
                spellStartTime: spellStartTime,
                parentSubSpellStartTime: parentSubSpellStartTime,
                slaveMark: new UnityEnvironmentMark(UnityEnvironmentMark.ServerOrClient.Server),
                firstOrLast: false,
                canceled: false,
                modifiers: modifiers,
                context: null,
                repo: _repo
            );
        }

        public Stopwatch StartStopwatch()
        {
            return null;
        }

        public void StopStopwatch(ref Stopwatch sw, SpellWordDef word, string operation)
        {
            
        }

        public struct SpellAction
        {
            public SpellActionType Action;
            public SpellWordDef Word;
            public SpellWordCastData Data;
        }

        public enum SpellActionType
        {
            Start, Finish
        }
        
        #endregion
    }
}

internal static class Ext
{
    public static T Do<T>(this T self, Action<T> action)
    {
        action(self);
        return self;
    }
    
    public static T WaitTask<T>(this T self) where T : Task
    {
//        try
//        {
            self.Wait();
//        }
//        catch (AggregateException e)
//        {
//            if (self.Exception != null)
//                throw self.Exception.Flatten();
//            throw new Exception(e.InnerExceptions.First().Message);
//        }
        return self;
    }

}

