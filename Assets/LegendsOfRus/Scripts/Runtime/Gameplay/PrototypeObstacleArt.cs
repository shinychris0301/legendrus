using UnityEngine;
using UnityEngine.UI;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus
{
    public static class PrototypeObstacleArt
    {
        public static Sprite Load(BoardObstacleKind kind, int layers = 2)
        {
            string name = kind == BoardObstacleKind.Crate ? "crate" :
                kind == BoardObstacleKind.Ice ? "ice" : kind == BoardObstacleKind.Chain ? "chain" :
                kind == BoardObstacleKind.RunicSeal ? "seal" : null;
            return name == null || layers <= 0 ? null : PrototypeBoardArt.Load(name + (layers == 1 ? "_damaged" : ""));
        }
        public static void Apply(Image host, BoardObstacleKind kind, int layers, bool showDamage)
        {
            // Retire old procedural marks if this Image was reused from an earlier presentation.
            foreach (string name in new[] { "RunicSealGlyph", "DamageFissures" })
            {
                var old = host.transform.Find(name);
                if (old) old.gameObject.SetActive(false);
            }
            host.sprite = Load(kind, showDamage ? layers : 2);
            host.enabled = kind != BoardObstacleKind.None && layers > 0;
            // Intact frost reveals the gem; final-stage art has a genuinely open center.
            host.color = new Color(1, 1, 1, kind == BoardObstacleKind.Ice && showDamage ? (layers > 1 ? .52f : .8f) : 1);
        }
    }
}
