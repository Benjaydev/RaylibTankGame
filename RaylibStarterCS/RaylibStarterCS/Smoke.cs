using System;
using System.Collections.Generic;
using System.Text;

namespace RaylibStarterCS
{
    public class Smoke : SpriteObject
    {
        float LifeLength = .5f;
        float AliveTime = 0f;
        public Smoke()
        {
            Load("./PNG/Smoke/smokeOrange1.png");
        }

        public override void OnUpdate(float deltaTime)
        {
            AliveTime+=deltaTime;

            if(AliveTime >= LifeLength)
            {
                return;
            }
        }
    }
}
