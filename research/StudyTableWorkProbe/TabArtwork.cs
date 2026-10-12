// SPDX-License-Identifier: MPL-2.0
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

namespace NikichMods.Research
{
    internal static class TabArtwork
    {
        private static Sprite material;
        private static Sprite recipe;
        private static Texture2D materialTexture;
        private static Texture2D recipeTexture;

        internal static void Load()
        {
            try
            {
                material = Read("MaterialStudy", "a4fc4f332558ab283f7972d1c8a4ed64cb7273fd55e01368221f027ae9181e2b", out materialTexture);
                recipe = Read("RecipeResearch", "7675fedcec0c3aba2f558cbda3f6f6980ab3e62653d412be0045a73281ce02ff", out recipeTexture);
                StudyTableWorkProbe.Write("RAR_TAB_ART_READY canvas=40x26 embedded=True filter=Point");
            }
            catch { Dispose(); throw; }
        }

        private static Sprite Read(string name, string expectedHash, out Texture2D texture)
        {
            texture = null;
            byte[] bytes;
            using (var source = Assembly.GetExecutingAssembly().GetManifestResourceStream("RAR." + name + ".png"))
            using (var buffer = new MemoryStream())
            {
                if (source == null) throw new InvalidOperationException("Missing embedded tab artwork: " + name);
                source.CopyTo(buffer);
                bytes = buffer.ToArray();
            }
            using (var sha = SHA256.Create())
                if (BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant() != expectedHash)
                    throw new InvalidOperationException("Unexpected tab artwork bytes: " + name);
            texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.name = "RAR " + name;
            if (!ImageConversion.LoadImage(texture, bytes, true) || texture.width != 40 || texture.height != 26)
                throw new InvalidOperationException("Invalid tab artwork canvas: " + name);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            return Sprite.Create(texture, new Rect(0, 0, 40, 26), new Vector2(0.5f, 0.5f), 1f, 0, SpriteMeshType.FullRect);
        }

        internal static Sprite Get(bool isMaterial)
        {
            var sprite = isMaterial ? material : recipe;
            if (sprite == null) throw new InvalidOperationException("Tab artwork is not loaded");
            return sprite;
        }

        internal static void Dispose()
        {
            if (material != null) UnityEngine.Object.Destroy(material);
            if (recipe != null) UnityEngine.Object.Destroy(recipe);
            if (materialTexture != null) UnityEngine.Object.Destroy(materialTexture);
            if (recipeTexture != null) UnityEngine.Object.Destroy(recipeTexture);
            material = null; recipe = null;
            materialTexture = null; recipeTexture = null;
        }
    }
}
