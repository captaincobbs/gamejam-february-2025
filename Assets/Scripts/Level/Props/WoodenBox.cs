namespace Assets.Scripts.Level.Props
{
    public class WoodenBox : Entity
    {
        public override void Move()
        {
            AudioManager.Instance.PlayOneShot(onMove, $"WoodenBox.{nameof(onMove)}");
        }

        public override void Kill()
        {
            AudioManager.Instance.PlayOneShot(onKill, $"WoodenBox.{nameof(onKill)}");
        }
    }
}