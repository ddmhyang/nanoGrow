using System.Collections.Generic;

namespace nanoGrow
{
    public class AnimationData
    {
        public Dictionary<string, List<string>> AnimationPaths { get; set; }
        // BackgroundImagePath는 더 이상 필요 없으므로 삭제합니다.
        // public string BackgroundImagePath { get; set; }

        public AnimationData()
        {
            AnimationPaths = new Dictionary<string, List<string>>
            {
                // ## PetState enum과 이름 및 대소문자를 일치시킵니다. ##
                { "Idle", new List<string>() },
                { "Eating", new List<string>() },
                { "Cleaning", new List<string>() },
                { "Washing", new List<string>() },
                { "Sleeping", new List<string>() },
                { "Playing", new List<string>() },
                { "Flying", new List<string>() },
                { "MovingLeft", new List<string>() },
                { "MovingRight", new List<string>() },
                { "Background", new List<string>() }
            };
        }
    }
}