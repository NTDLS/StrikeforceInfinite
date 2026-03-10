using Ae.Engine.Sprite;

namespace Ae.Engine.Interrogation._Superclass
{
    public interface IInterrogationForm
    {
        public void StartWatch(AeEngine engine, IAeSprite sprite);
        public void WriteLine(string text, System.Drawing.Color color);
        public void Write(string text, System.Drawing.Color color);
        public void ClearText();
        public void Show();
        public void Hide();
    }
}
