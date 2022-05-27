using YaEngine.Render;

namespace YaDemo
{
    public class ColorShader
    {
        public static readonly ShaderInitializer Value = new("ColorShader",
            new StringShaderProvider(Vertex),
            new StringShaderProvider(Fragment));
        
        private const string Vertex = @"
#version 330 core
in vec3 vPos;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform vec4 uColor;

out vec4 fColor;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(vPos, 1.0);
    fColor = uColor;
}
";

        private const string Fragment = @"
#version 330 core

in vec4 fColor;
out vec4 FragColor;

void main()
{
    FragColor = fColor;
}
";
    }
}