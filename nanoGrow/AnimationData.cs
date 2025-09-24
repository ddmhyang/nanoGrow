using System.Collections.Generic;

namespace nanoGrow
{
    public class AnimationData
    {
        public Dictionary<string, List<string>> AnimationPaths { get; set; }

        public AnimationData()
        {
            AnimationPaths = new Dictionary<string, List<string>>
            {
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

        // ## 이 함수가 핵심입니다! ##
        public List<string> GetPathsForState(PetState state)
        {
            string stateKey = state.ToString();

            // 1. 사용자 설정에 유효한 경로가 있는지 확인
            if (AnimationPaths.TryGetValue(stateKey, out List<string>? customPaths) && customPaths.Count > 0 && !string.IsNullOrEmpty(customPaths[0]))
            {
                return customPaths; // 있으면 사용자 설정 경로 반환
            }

            // 2. 없으면, 상태에 맞는 기본 경로를 반환
            switch (stateKey)
            {
                case "MovingLeft":
                    return new List<string> { "/Images/move_left1.png", "/Images/move_left2.png" };
                case "MovingRight":
                    return new List<string> { "/Images/move_right1.png", "/Images/move_right2.png" };
                case "Idle":
                default: // Idle 및 기타 모든 미설정 상태는 Idle 기본 애니메이션을 보여줌
                    return new List<string> { "/Images/pet_idle.png", "/Images/pet_idle2.png" };
            }
        }
    }
}