using YaEngine.Render;

namespace YaDemo
{
    public class UnlitTextureShader
    {
        public static readonly ShaderInitializer Value = new(nameof(UnlitTextureShader),
            new StringShaderProvider(Vertex),
            new StringShaderProvider(Fragment));
        
        private const string Vertex = @"
#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 2) in vec2 vUv;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec2 fUv;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(vPos,1.0f);
    fUv = vUv;
}
";

        private const string Fragment = @"
#version 330 core

in vec2 fUv;

uniform sampler2D uTexture0;

out vec4 FragColor;

void main()
{
      FragColor = texture(uTexture0, fUv);
}
";
    }
}