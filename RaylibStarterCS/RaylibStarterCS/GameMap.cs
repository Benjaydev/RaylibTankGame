using System;
using System.Collections.Generic;
using System.Text;
using MathsClasses;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace RaylibStarterCS
{
    public class GameMap
    {

        SpriteObject sandbagBeigeS;
        SpriteObject sandbagBrownS;

        SpriteObject barrelRedS;
        SpriteObject barrelGreenS;
        SpriteObject barrelGreyS;

        public GameMap()
        {
            // Create sprites for sandbags
            sandbagBeigeS = CreateSandbagSpriteTemplate("sandbagBeige");
            sandbagBrownS = CreateSandbagSpriteTemplate("sandbagBrown");

            // Create sprites for barrels
            barrelGreenS = CreateBarrelSpriteTemplate("barrelGreen");
            barrelRedS = CreateBarrelSpriteTemplate("barrelRed");
            barrelGreyS = CreateBarrelSpriteTemplate("barrelGrey");

            // ------ Create building on bottom left ------
            // Main body of building
            CornerSandbagStructure(4, 325, GetScreenHeight()-425, 90);
            LineSandbagStructure(8, 180, GetScreenHeight()-195, 0);
            // The door way edges against walls
            LineSandbagStructure(1, 0, GetScreenHeight() - 435, 0);
            LineSandbagStructure(2, 515, GetScreenHeight(), -90);
            // Corner of building
            CornerSandbagStructure(2, GetScreenWidth()-200, GetScreenHeight()-150, 180);
            // ------------

            // Spawn barrels in different areas of map
            SpawnRandomBarrels(10, 400, 50, GetScreenWidth()-250, GetScreenHeight()-250);
            SpawnRandomBarrels(3, 50, 50, 350, 200);
            SpawnRandomBarrels(3, 50, GetScreenHeight() - 100, 200, GetScreenHeight() - 50);

            // Create lighting inside building
            CreateLamp(250, 320, new Light(new Vector3(200, 149, 151), 0.3f, .3f, new Color(150, 150, 255, 255)));
            CreateLamp(430, 650, new Light(new Vector3(200, 149, 151), 0.3f, .3f, new Color(150, 150, 255, 255)));
            CreateLamp(35, 500, new Light(new Vector3(150, 149, 151), 0.3f, .3f, new Color(150, 150, 255, 255)));

            // Create fire pits around the map
            CreateFirePit(30, 30, new Light(new Vector3(400, 200, 400), 0.5f, .5f, new Color(255, 150, 0, 255)));
            CreateFirePit(1070, 80, new Light(new Vector3(300, 200, 300), 0.5f, .5f, new Color(255, 150, 0, 255)));
            CreateFirePit(940, 480, new Light(new Vector3(300, 200, 300), 0.5f, .5f, new Color(255, 150, 0, 255)));
        }

        // Create lamp object which has custom light
        public void CreateLamp(int x, int y, Light light)
        {
            SceneObject lightObject = new SceneObject();
            // Create sprite
            SpriteObject lightSprite = new SpriteObject();
            lightSprite.Load("./PNG/Obstacles/light.png");
            lightSprite.SetPosition(-lightSprite.Width / 2, -lightSprite.Height / 2);
            lightObject.AddChild(lightSprite);

            // Add light to object
            lightObject.AddChild(light);
            
            // Setup object
            lightObject.SetPosition(x, y);
            lightObject.hasCollision = true;
            lightObject.movable = true;
            lightObject.HitWidth = lightSprite.Width;
            lightObject.SetCollisionType(new CircleCollider(new Vector3(0, 0, 0), lightObject.HitWidth));
            lightObject.tag = "CollideAll";

            // Add to scene
            lightObject.AddSelfToSceneObjects();
        }

        // Create fire pit object which has custom light
        public void CreateFirePit(int x, int y, Light light)
        {
            SceneObject lightObject = new SceneObject();
            // Create sprite
            SpriteObject lightSprite = new SpriteObject();
            lightSprite.Load("./PNG/Obstacles/firePit.png");
            lightSprite.SetPosition(-lightSprite.Width / 2, -lightSprite.Height / 2);
            lightObject.AddChild(lightSprite);

            // Add light to object
            lightObject.AddChild(light);

            // Setup object
            lightObject.SetPosition(x, y);
            lightObject.hasCollision = true;
            lightObject.HitWidth = lightSprite.Width;
            lightObject.SetCollisionType(new CircleCollider(new Vector3(0, 0, 0), lightObject.HitWidth));
            lightObject.tag = "CollidePlayer";

            // Add to scene
            lightObject.AddSelfToSceneObjects();
        }



        // Create a sandbag sceneobject to be used in structure making functions
        public SceneObject CreateSandbagObjectTemplate(SpriteObject sandbagS, int posX = 0, int posY = 0)
        {
            // Setup sandbag object
            SceneObject sandbag = new SceneObject();
            sandbag.AddChild(sandbagS);
            sandbag.hasCollision = true;
            sandbag.tag = "CollideAll";
            sandbag.SetPosition(posX, posY);
            sandbag.HitWidth = sandbagS.Width;
            sandbag.HitHeight = sandbagS.Height;

            return sandbag;
        }

        // Create sandbag sprite
        public SpriteObject CreateSandbagSpriteTemplate(string type = "sandbagBeige")
        {
            // Make sandbag sprite
            SpriteObject sandbagS = new SpriteObject();
            sandbagS.Load($"./PNG/Obstacles/{type}.png");
            sandbagS.SetPosition(-(sandbagS.Width / 2), -(sandbagS.Height / 2));
            sandbagS.Scale(0.75f, 1);
            return sandbagS;
        }
        
        // Create barrel sprite
        public SpriteObject CreateBarrelSpriteTemplate(string type = "barrelGreen")
        {
            // Setup barrel sprite
            SpriteObject barrelSprite = new SpriteObject();
            barrelSprite.Load($"./PNG/Obstacles/{type}_up.png");
            barrelSprite.SetPosition(-(barrelSprite.Width / 2), -(barrelSprite.Height / 2));
            return barrelSprite;
        }

        // Return a random sandbag sprite
        public SpriteObject GetRandomSandbagSprite()
        {
            switch (Game.gameRandom.Next(0, 2))
            {
                case 0:
                    return sandbagBeigeS;
                case 1:
                    return sandbagBrownS;
                default:
                    return sandbagBeigeS;
            }
        }
        
        // Return a random barrel sprite
        public SpriteObject GetRandomBarrelSprite()
        {
            switch (Game.gameRandom.Next(0, 3))
            {
                case 0:
                    return barrelGreenS;
                case 1:
                    return barrelRedS;
                case 2:
                    return barrelGreyS;
                default:
                    return barrelGreenS;
            }
        }

        /// <summary>
        /// Create a corner sandbag structure and return it. Default corner formation is top left
        /// </summary>
        public void CornerSandbagStructure(int size, int posX = 0, int posY = 0, int rotDegrees = 0)
        {
            SceneObject sandBagStructure = new SceneObject();
            sandBagStructure.SetPosition(posX, posY);

            // Get random sandbag sprite
            SpriteObject sandbagS = GetRandomSandbagSprite();

            // Horizontal part
            SceneObject sandbagC = new SceneObject(CreateSandbagObjectTemplate(sandbagS, posX, posY));
            sandbagC.SetPosition(0, 0);
            sandBagStructure.AddChild(sandbagC);

            // Create copies of sandbag
            for (int i = 0; i < size; i++)
            {
                // Get new random sandbag sprite
                sandbagS = GetRandomSandbagSprite();

                // Horizontal part
                SceneObject sandbagW = new SceneObject(CreateSandbagObjectTemplate(sandbagS, posX, posY));
                sandbagW.SetPosition(sandbagS.Width * (i+1), 0);

                // Get new random sandbag sprite
                sandbagS = GetRandomSandbagSprite();

                // Vertical part
                SceneObject sandbagH = new SceneObject(CreateSandbagObjectTemplate(sandbagS, posX, posY));
                sandbagH.SetRotate(90 * DEG2RAD);
                sandbagH.SetPosition(0 - (sandbagS.Width / 6), (sandbagS.Height / 6) + sandbagS.Height * (i + 1));

                sandBagStructure.AddChild(sandbagW);
                sandBagStructure.AddChild(sandbagH);

            }
            // Apply rotation to structure
            sandBagStructure.Rotate(rotDegrees * DEG2RAD);
            // Add to scene
            sandBagStructure.AddSelfToSceneObjects();
          
        }

        /// <summary>
        /// Create a line sandbag structure and add it to scene.
        /// </summary>
        public void LineSandbagStructure(int size, int posX = 0, int posY = 0, int rotDegrees = 0)
        {
            SceneObject sandBagStructure = new SceneObject();
            sandBagStructure.SetPosition(posX, posY);
            // Create copies of sandbag
            for (int i = 0; i < size; i++)
            {
                // Get random sanbag sprite
                SpriteObject sandbagS = GetRandomSandbagSprite();

                SceneObject sandbagW = new SceneObject(CreateSandbagObjectTemplate(sandbagS, posX, posY));
                sandbagW.SetPosition(sandbagS.Width * (i), 0);

                sandBagStructure.AddChild(sandbagW);

            }
            // Apply rotation to structure
            sandBagStructure.Rotate(rotDegrees * DEG2RAD);
            // Add to scene
            sandBagStructure.AddSelfToSceneObjects();

        }



        // Spawn barrels randomly within area
        public void SpawnRandomBarrels(int amount, int minX = 0, int minY = 0, int maxX = 200, int maxY = 200)
        {
            // Setup barrel obstacles
            for (int i = 0; i < amount; i++)
            {
                // Get random barrel sprite
                SpriteObject barrelSprite = GetRandomBarrelSprite();


                // Choose random point on screen
                int randomX = Game.gameRandom.Next(minX, maxX);
                int randomY = Game.gameRandom.Next(minY, maxY);

                // Vary size slightly
                float randomSize = (1f + (float)Game.gameRandom.NextDouble())/1.5f;

                // Setup barrel object
                SceneObject barrel = new SceneObject();
                SpriteObject spriteCopy = new SpriteObject(barrelSprite);
                spriteCopy.Scale(randomSize, randomSize);
                spriteCopy.SetPosition(-(spriteCopy.Width / 2), -(spriteCopy.Height / 2));
                barrel.AddChild(spriteCopy);
                barrel.hasCollision = true;
                barrel.tag = "CollideAll";
                barrel.movable = true;
                barrel.SetPosition(randomX, randomY);
                barrel.HitWidth = spriteCopy.Width;
                barrel.HitHeight = spriteCopy.Height;
                barrel.SetCollisionType(new CircleCollider(new Vector3(0, 0, 0), barrel.HitWidth));

                // Add to scene
                barrel.AddSelfToSceneObjects();
            }
        }
    }
}
