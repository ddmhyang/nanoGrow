using System.Collections.Generic;

namespace nanoGrow // 실제 프로젝트 이름에 맞춰주세요.
{
    public class AnimationData
    {
        // {"idle", ["path1.png", "path2.png"]}, {"fly", ["path3.png", "path4.png"]} ...
        public Dictionary<string, List<string>> AnimationPaths { get; set; }

        public string BackgroundImagePath { get; set; } // 배경 이미지 경로 추가

        public AnimationData()
        {
            AnimationPaths = new Dictionary<string, List<string>>
            {
                { "idle", new List<string>() },
                { "fly", new List<string>() },
                { "sleep", new List<string>() },
                { "eat", new List<string>() },
                { "play", new List<string>() },
                { "movingleft", new List<string>() },
                { "movingright", new List<string>() },
                { "wash", new List<string>() },
                { "background", new List<string>() } // background 키 추가
            };
            // BackgroundImagePath 속성은 이제 AnimationPaths["background"]에 통합되므로 삭제해도 됩니다.
        }
    }
}