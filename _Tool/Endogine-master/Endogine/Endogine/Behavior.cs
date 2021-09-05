using System;

namespace Endogine
{
    /// <summary>
    /// Summary description for Behavior.
    /// </summary>
    public class Behavior
    {
        //TODO: remove behaviors - use child sprites that control the parent instead!!
        protected Sprite m_sp = null;
        //private string _name;

        public Behavior(Sprite a_sp)
        {
            this.Sprite = a_sp;
        }

        public Behavior()
        {
        }

        public virtual Sprite Sprite
        {
            set
            {
                this.RemoveFromSprite();
                this.m_sp = value;
                if (this.m_sp == null)
                    return;

                if (!this.m_sp.HasBehavior(this))
                    this.m_sp.AddBehavior(this);
                this.m_sp.MouseEvent += new Endogine.Sprite.MouseEventDelegate(Mouse);
                this.m_sp.EnterFrameEvent += new Endogine.Sprite.EnterFrameEventDelegate(EnterFrame);
            }
            get
            {
                return this.m_sp;
            }
        }

        private void RemoveFromSprite()
        {
            if (this.m_sp == null)
                return;
            this.m_sp.MouseEvent -= new Endogine.Sprite.MouseEventDelegate(Mouse);
            this.m_sp.EnterFrameEvent -= new Endogine.Sprite.EnterFrameEventDelegate(EnterFrame);
            this.m_sp.RemoveBehavior(this);
        }

        public virtual void Dispose()
        {
            this.RemoveFromSprite();
        }

        protected virtual void Mouse(Sprite sender, System.Windows.Forms.MouseEventArgs e, Endogine.Sprite.MouseEventType t)
        {

        }

        protected virtual void EnterFrame()
        {

        }
    }
}
