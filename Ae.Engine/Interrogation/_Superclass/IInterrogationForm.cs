using Ae.Library.Sprite;

namespace Ae.Engine.Interrogation._Superclass
{
    public interface IInterrogationForm
    {
        public void StartWatch(AeEngine engine, ISprite sprite);
        public void WriteLine(string text, System.Drawing.Color color);
        public void Write(string text, System.Drawing.Color color);
        public void ClearText();
        public void Show();
        public void Hide();
    }
}
