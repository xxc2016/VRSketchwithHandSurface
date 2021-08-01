namespace VRPainting
{
    public enum ActionState
    {
        FREE,
        PAINT,
        GIZMOS,
        UI,
        SURFACE,
        DELETE,
    }
    public enum PaintingMode
    {
        FREEMODE,
        STRAIGHTMODE,
        CIRCLEMODE,
        //DELETEMODE,
    }

    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    public enum SurfaceMode
    {
        ONESTROKE,
        ROTATE,
        SPHERE,
        PLANE,
    }
}