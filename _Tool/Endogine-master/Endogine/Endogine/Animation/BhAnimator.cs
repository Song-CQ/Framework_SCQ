using System;
using System.Collections.Generic;
//using System.Collections;
using System.Text;

namespace Endogine.Animation
{
    public class BhAnimator : Sprite
	{
		public delegate void AnimDelegate(object sender, string eventName);
		public event AnimDelegate AnimationEvent;

		private List<string> _picRefNames;
		private string _currentName;
		//private float _fPosition;
		//public float StepSize = 1;
		//private ArrayList _displayOrder;

        private Animator _animator;

        public BhAnimator()
		{
            this.Prepare();
		}

        public BhAnimator(Sprite animateThis, string frameSetName)
        {
            this.Prepare();

            this.Parent = animateThis;
            List<string> frameSet = EH.Instance.CastLib.FrameSets[frameSetName];
            this.FrameSet = frameSetName;
        }

        private void Prepare()
        {
            this.Name = "_anim";
            this._animator = new Animator(this, "Index");
            this._animator.Mode = Animator.Modes.Loop;
        }

        public override void Dispose()
        {
            this._animator.Dispose();
            base.Dispose();
        }

        public Animator Animator
        {
            get { return this._animator; }
        }
        public int Index
        {
            get { return 0; }
            set
            {
                this.Parent.PicRefName = this._picRefNames[value];
            }
        }

		public string FrameSet
		{
			set
			{
				if (value == this._currentName)
					return;
				//EH.Put("Newframeset "+value);

				this._picRefNames = EH.Instance.CastLib.FrameSets[value];
				if (this._picRefNames==null)
					throw new Exception("FrameSet doesn't exist: "+ value);

				this._currentName = value;


                List<float> order = new List<float>();
				for (int i=0; i<this._picRefNames.Count;i++)
                    order.Add(i);
                //add one extra at the end, otherwise it won't show (e.g. keyframes 0,1,1)
                order.Add(this._picRefNames.Count-1);
                this._animator.SetAnimationList(order);
			}
			get {return this._currentName;}
		}

		public void SetAnimation(string s)
		{
			string[] ss = s.Split(',');

            List<float> order = new List<float>();
            foreach (string sFrame in ss)
                order.Add(Convert.ToSingle(sFrame));
            this._animator.SetAnimationList(order);

            //if (this._fPosition >= this._frameNumbers.Count)
            //    this._fPosition = 0;
			//Endogine.Animation.AnimationHelpers.ParseAnimationString(s);
		}
    }
}
