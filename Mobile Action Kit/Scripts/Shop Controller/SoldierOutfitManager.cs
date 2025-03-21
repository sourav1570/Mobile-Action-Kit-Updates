using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class SoldierOutfitManager : MonoBehaviour
    {
        [TextArea]
        public string scriptInfo = "This script allows customization of soldier outfits by assigning new materials to SkinnedMeshRenderers and MeshRenderers. It provides predefined methods for applying different attire.";

        [Tooltip("List of outfits that can be applied to the soldier.")]
        public List<AllAttires> AddOutfits = new List<AllAttires>();

        [System.Serializable]
        public class AllAttires
        {
            [Tooltip("Material to be applied to this outfit.")]
            public Material OutfitMaterial;

            [Tooltip("Skinned mesh renderers that will use this material.")]
            public SkinnedMeshRenderer[] AllSkinnedMeshRenderers;

            [Tooltip("Mesh renderers that will use this material.")]
            public MeshRenderer[] AllMeshRenderers;
        }

        public void Attire1()
        {
            for (int x = 0; x < AddOutfits[0].AllSkinnedMeshRenderers.Length; x++)
            {
                AddOutfits[0].AllSkinnedMeshRenderers[x].material = AddOutfits[0].OutfitMaterial;
            }
            for (int x = 0; x < AddOutfits[0].AllMeshRenderers.Length; x++)
            {
                AddOutfits[0].AllMeshRenderers[x].material = AddOutfits[0].OutfitMaterial;
            }
        }
        public void Attire2()
        {
            for (int x = 0; x < AddOutfits[1].AllSkinnedMeshRenderers.Length; x++)
            {
                AddOutfits[1].AllSkinnedMeshRenderers[x].material = AddOutfits[1].OutfitMaterial;
            }
            for (int x = 0; x < AddOutfits[1].AllMeshRenderers.Length; x++)
            {
                AddOutfits[1].AllMeshRenderers[x].material = AddOutfits[1].OutfitMaterial;
            }
        }
        public void Attire3()
        {
            for (int x = 0; x < AddOutfits[2].AllSkinnedMeshRenderers.Length; x++)
            {
                AddOutfits[2].AllSkinnedMeshRenderers[x].material = AddOutfits[2].OutfitMaterial;
            }
            for (int x = 0; x < AddOutfits[2].AllMeshRenderers.Length; x++)
            {
                AddOutfits[2].AllMeshRenderers[x].material = AddOutfits[2].OutfitMaterial;
            }
        }
        public void Attire4()
        {
            for (int x = 0; x < AddOutfits[3].AllSkinnedMeshRenderers.Length; x++)
            {
                AddOutfits[3].AllSkinnedMeshRenderers[x].material = AddOutfits[3].OutfitMaterial;
            }
            for (int x = 0; x < AddOutfits[3].AllMeshRenderers.Length; x++)
            {
                AddOutfits[3].AllMeshRenderers[x].material = AddOutfits[3].OutfitMaterial;
            }
        }
        public void Attire5()
        {
            for (int x = 0; x < AddOutfits[4].AllSkinnedMeshRenderers.Length; x++)
            {
                AddOutfits[4].AllSkinnedMeshRenderers[x].material = AddOutfits[4].OutfitMaterial;
            }
            for (int x = 0; x < AddOutfits[4].AllMeshRenderers.Length; x++)
            {
                AddOutfits[4].AllMeshRenderers[x].material = AddOutfits[4].OutfitMaterial;
            }
        }
        public void Attire6()
        {
            for (int x = 0; x < AddOutfits[5].AllSkinnedMeshRenderers.Length; x++)
            {
                AddOutfits[5].AllSkinnedMeshRenderers[x].material = AddOutfits[5].OutfitMaterial;
            }
            for (int x = 0; x < AddOutfits[5].AllMeshRenderers.Length; x++)
            {
                AddOutfits[5].AllMeshRenderers[x].material = AddOutfits[5].OutfitMaterial;
            }
        }
    }
}