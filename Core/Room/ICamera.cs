namespace GMLib;

public interface ICamera
{
    public Matrix4x4 ViewMatrix { get; set; }
    
    public Matrix4x4 ProjectionMatrix { get; set; }

    public float Angle { get; set; }
    
    public Rectangle View { get; set; }

    public Matrix4x4 BuildMatrix();
    
    public void UpdateMatrix();
    
    public void Update();
    
    public void Begin();
    
    public void End();
}