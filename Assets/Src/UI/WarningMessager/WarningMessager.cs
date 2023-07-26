using System.Text;
using Assets.Src.ResourceSystem;
using JetBrains.Annotations;
using UnityEngine;
using Uins.Sound;

namespace Uins
{
    public class WarningMessager : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private MessagesUnit _hiMessagesUnit;

        [SerializeField, UsedImplicitly]
        private MessagesUnit _loMessagesUnit;

        [SerializeField, UsedImplicitly]
        private Color _errorMessageColor;

        [SerializeField, UsedImplicitly]
        private Sprite _errorMessageSprite;

        public Sprite[] TestSprites;

        [SerializeField, UsedImplicitly]
        private TechPointDefRef _techPointDefRef;

        [SerializeField, UsedImplicitly]
        private ScienceDefRef[] _scienceDefRefs;

        public static WarningMessager Instance;


        //=== Unity ===============================================================

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);

            _hiMessagesUnit.AssertIfNull(nameof(_hiMessagesUnit));
            _loMessagesUnit.AssertIfNull(nameof(_loMessagesUnit));
            _errorMessageSprite.AssertIfNull(nameof(_errorMessageSprite));

            //TestSprites.IsNullOrEmptyOrHasNullElements(nameof(TestSprites));
            _techPointDefRef.Target.AssertIfNull(nameof(_techPointDefRef));
            _scienceDefRefs.IsNullOrEmptyOrHasNullElements(nameof(_scienceDefRefs));
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }


        //=== Public ==============================================================

        public void ShowWarningMessage(string message, Color textColor, Sprite sprite1 = null, Sprite sprite2 = null,
            Color spritesColor = new Color(), bool isHiPosition = true, object someInfo = null)
        {
//            UI.CallerLogInfo($"'{message}' {someInfo} col={GetColorInfo(textColor)}, isHiPosition{isHiPosition.AsSign()}, " +
//                         $"sprites={sprite1}/{sprite2} col={GetColorInfo(spritesColor)}"); //DEBUG
            SoundControl.Instance.KnowledgeBaseNewNote?.Post(gameObject.transform.root.gameObject);
            var messagesUnit = isHiPosition ? _hiMessagesUnit : _loMessagesUnit;
            messagesUnit.ShowWarningMessage(message, textColor, sprite1, sprite2, spritesColor, someInfo);
        }

        public void ShowErrorMessage(string errorMessage)
        {
            ShowWarningMessage(errorMessage, _errorMessageColor, _errorMessageSprite, null, _errorMessageColor, false);
        }


        //=== Private =========================================================

        private string GetColorInfo(Color color)
        {
            return color.Equals(default(Color)) ? "default" : ((Color32)color).ToString();
        }

        [UsedImplicitly]
        public void ShowRandomMessage_Debug(bool isHiPosition) //DEBUG
        {
            var msgParts = new[] {"One", "Two\n", "Three 4 Five\n", "Random\n", "Message", "..."};
            var wordCount = Random.Range(1, 8);
            var sb = new StringBuilder();

            for (int i = 0; i < wordCount; i++)
                sb.Append($"{msgParts[Random.Range(0, msgParts.Length)]} ");

            var message = sb.ToString().TrimEnd(' ');
            var color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1);
            int sprite1Idx = Random.Range(0, TestSprites.Length);
            int sprite2Idx = Random.Range(0, TestSprites.Length);

            ShowWarningMessage(message, color, TestSprites[sprite1Idx], TestSprites[sprite2Idx], Color.white, isHiPosition);
        }
    }
}