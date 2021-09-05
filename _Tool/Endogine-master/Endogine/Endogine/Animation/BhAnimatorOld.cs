//using System;
//using System.Collections;

//namespace Endogine.Animation
//{
//    /// <summary>
//    /// Summary description for BhAnimator.
//    /// </summary>
//    public class BhAnimator : Behavior
//    {
//        private string _animationName;
//        private SortedList _animNameToMemberAndAnimations;
//        private SortedList _memberToAnims;

//        public BhAnimator()
//        {
//            this._memberToAnims = new SortedList();
//            this._animNameToMemberAndAnimations = new SortedList();
//        }

//        public void AddMemberAndAnimations(MemberSpriteBitmap mb, SortedList anims)
//        {
//            this._memberToAnims.Add(mb, anims);
//            for (int i=0; i<anims.Count; i++)
//            {
//                Hashtable info = new Hashtable();
//                info.Add("Member", mb);
//                info.Add("Animation", anims.GetByIndex(i));
//                this._animNameToMemberAndAnimations.Add((string)anims.GetKey(i), info);
//            }
//        }

//        public void AddXmlForMember(string fileName, MemberSpriteBitmap mb)
//        {
//            SortedList sl = AnimationHelpers.ParseAnimations(fileName);
//            this.AddMemberAndAnimations(mb, sl);
//        }

//        public string Animation
//        {
//            set
//            {
//                Hashtable info = (Hashtable)this._animNameToMemberAndAnimations[value];
//                this.m_sp.Member = (MemberSpriteBitmap)info["Member"];
//                this.m_sp.AutoAnimator.AnimationList = (SortedList)info["Animation"];
//                this.m_sp.AutoAnimator.Position = 0;
//                this.m_sp.AutoAnimator.Mode = Endogine.Animation.Animator.Modes.Loop;
//            }
//        }

////		protected override void EnterFrame()
////		{
////			base.EnterFrame ();
////		}
//    }
//}
