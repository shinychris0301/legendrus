using System;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    public sealed class GoalConfig
    {
        public int red = 15;
        public int blue = 18;
    }

    [Serializable]
    public sealed class ChapterRuntimeConfig
    {
        public int chapterId = 1;
        public string name = "Родная Деревня";
        public int moves = 28;
        public GoalConfig goals = new GoalConfig();

        public static ChapterRuntimeConfig LoadChapter1()
        {
            var text = Resources.Load<TextAsset>("Config/chapter01");
            if (text == null)
                return new ChapterRuntimeConfig();

            try
            {
                return JsonUtility.FromJson<ChapterRuntimeConfig>(text.text) ?? new ChapterRuntimeConfig();
            }
            catch
            {
                return new ChapterRuntimeConfig();
            }
        }
    }
}
