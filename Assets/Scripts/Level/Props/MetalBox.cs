public class MetalBox : Entity
{
    public override void Move()
    {
        AudioManager.Instance.PlayOneShot(onMove, $"MetalBox.{nameof(onMove)}");
    }

    public override void Kill()
    {
        AudioManager.Instance.PlayOneShot(onKill, $"MetalBox.{nameof(onKill)}");
    }
}
