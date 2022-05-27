using YaEngine.Render;

namespace YaDemo
{
    public class DiffuseShader
    {
        public static readonly ShaderInitializer Value = new(nameof(DiffuseShader),
            new StringShaderProvider(Vertex),
            new StringShaderProvider(Fragment));
        
        private const string Vertex = @"
#version 330 core
in vec3 vPos;
in vec2 vUv;
in vec3 vNormal;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

uniform vec3 lightColor;
uniform vec3 lightDirection;

out vec3 fPos;
out vec2 fUv;
out vec4 fDiffuse;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(vPos,1.0f);

    vec3 normal = normalize(mat3(transpose(inverse(uModel))) * vNormal);
    float diff = max(dot(normal, normalize(lightDirection)), 0.0);
    fDiffuse = vec4(diff * lightColor, 1.0);

    fPos = vec3(uModel * vec4(vPos,1.0f));
    fUv = vUv;
}
";

        private const string Fragment = @"
#version 330 core

in vec2 fUv;
in vec3 fNormal;
in vec3 fPos;
in vec4 fDiffuse;

uniform sampler2D uTexture0;

out vec4 FragColor;

void main()
{
      vec4 objectColor = texture(uTexture0, fUv);
      vec4 result = fDiffuse * objectColor;

      FragColor = result;
}
";
    }
}