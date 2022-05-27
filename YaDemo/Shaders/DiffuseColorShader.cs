using YaEngine.Render;

namespace YaDemo
{
    public class DiffuseColorShader
    {
        public static readonly ShaderInitializer Value = new(nameof(DiffuseColorShader),
            new StringShaderProvider(Vertex),
            new StringShaderProvider(Fragment));
        
        private const string Vertex = @"
#version 330 core
in vec3 vPos;
in vec3 vNormal;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

uniform vec3 lightColor;
uniform vec3 lightDirection;

out vec4 fDiffuse;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(vPos,1.0f);
    vec3 normal = normalize(mat3(transpose(inverse(uModel))) * vNormal);
    float diff = max(dot(normal, normalize(lightDirection)), 0.0);
    fDiffuse = vec4(diff * lightColor, 1.0);
}
";

        private const string Fragment = @"
#version 330 core

in vec4 fDiffuse;

uniform vec4 uColor;

out vec4 FragColor;

void main()
{
      vec4 objectColor = uColor;
      vec4 result = fDiffuse * objectColor;

      FragColor = result;
}
";
    }
}