using Modding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Archiv {
    public class Archiv: Mod {
        new public string GetName() => "Archiv";
        public override string GetVersion() => "1.0.0.0";

        static GameObject objectTemplate;
        GameObject boundsRef;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            objectTemplate = preloadedObjects["Town"]["grimm_tents/main_tent/Grimm_town_signs_0001_1"];
            On.TransitionPoint.OnTriggerEnter2D += transitionOverride;
            On.GameManager.OnNextLevelReady += sceneLoad;
        }

        public override List<(string, string)> GetPreloadNames() {
            return new List<(string, string)> { ("Town", "grimm_tents/main_tent/Grimm_town_signs_0001_1") };//don't mind me, just reusing golf code here
        }

        private void transitionOverride(On.TransitionPoint.orig_OnTriggerEnter2D orig, TransitionPoint self, Collider2D movingObj) {
            if(self.gameObject.scene.name == "Fungus3_archive" && self.name == "bot1") {
                if(movingObj.gameObject.layer == 9 && GameManager.instance.gameState == GlobalEnums.GameState.PLAYING) {
                    self.gameObject.SetActive(false);
                    boundsRef.SetActive(false);
                    awaitMemage();
                }
            }
            else {
                orig(self, movingObj);
            }
        }

        private void sceneLoad(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            try {
                if(boundsRef == null) {
                    boundsRef = GameObject.Find("Bounds Cage");
                }
                boundsRef.SetActive(true);
            }
            catch(Exception) { }
        }

        private async void awaitMemage() {
            while(HeroController.instance.gameObject.transform.position.y > -50) {
                await Task.Yield();
            }
            addMeme();
        }

        private async void addMeme() {
            const float y = 8.3f;
            const float z = -3.5f;
            GameObject kenobi = GameObject.Instantiate(objectTemplate, new Vector3(68.6f, y, z), Quaternion.identity);
            kenobi.name = "kenobi";
            SpriteRenderer sr = kenobi.GetComponent<SpriteRenderer>();
            Texture2D testMemeTexture = new Texture2D(1, 1);
            using(Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Archiv.Resources.meme.jpg")) {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                testMemeTexture.LoadImage(bytes, false);
                testMemeTexture.name = "kenobi meme";
            }
            var testMeme = Sprite.Create(testMemeTexture, new Rect(0, 0, testMemeTexture.width, testMemeTexture.height), new Vector2(0.5f, 0.5f), 64, 0, SpriteMeshType.FullRect);
            sr.sprite = testMeme;
            kenobi.transform.SetScaleMatching(0.371f);
            kenobi.SetActive(true);
            while(true) {
                try {
                    kenobi.transform.position = new Vector3(HeroController.instance.transform.position.x, y, z);
                    await Task.Yield();
                }
                catch(Exception) {
                    break;
                }
            }
        }
    }
}