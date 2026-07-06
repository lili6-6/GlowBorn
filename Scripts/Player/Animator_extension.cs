using UnityEngine;
using MoreMountains;
using MoreMountains.CorgiEngine;

namespace shootstar
{


    public class Animator_extension : CharacterAbility
    {
        //public enum AnimatorLayer
        //{
        //    Blank = 0,
        //    WhiteForm = 1,
        //    RedForm = 2,
        //    BlueForm = 3,
        //    GreenForm = 4
        //}
        public enum AnimatorLayer
        {
            WhiteForm = 0,
            RedForm = 1,
            BlueForm = 2,
            GreenForm = 3
        }

        [HideInInspector]public AnimatorLayer currentLayer = AnimatorLayer.WhiteForm;
        private int[] LayerIndex = { 0, 1, 2, 3};
        protected override void Start()
        {
            base.Start();
            InitializeLayers();
            ChangeLayer(AnimatorLayer.WhiteForm);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void InitializeLayers()
        {
            for(int i = 0; i < LayerIndex.Length; i++)
            {
                _animator.SetLayerWeight(LayerIndex[i], 0f);
            }
        }

        public void ChangeLayer(AnimatorLayer targetLayer)
        {
            if (currentLayer == targetLayer) return;

            _animator.SetLayerWeight((int)currentLayer, 0f);
            _animator.SetLayerWeight((int)targetLayer, 1f);
            currentLayer = targetLayer;
            Debug.Log(currentLayer);
        }
    }
}