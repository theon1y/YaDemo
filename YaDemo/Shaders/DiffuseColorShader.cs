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

out vec3 fPos;
out vec3 fNormal;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(vPos, 1);
    fPos = vec3(uModel * vec4(vPos, 1));
    fNormal = normalize(mat3(transpose(inverse(uModel))) * vNormal);
}
";

        private const string Fragment = @"
#version 330 core

in vec3 fPos;
in vec3 fNormal;

uniform vec4 uColor;

uniform vec3 uLightColor;
uniform vec3 uLightPosition;
uniform vec3 uViewPosition;

out vec4 FragColor;

void main()
{
    vec3 lightDirection = normalize(uLightPosition - fPos);
    float diff = max(dot(fNormal, lightDirection), 0);

    vec3 viewDirection = normalize(uViewPosition - fPos);
    vec3 reflectionDirection = reflect(-lightDirection, fNormal);
    float specularStrength = dot(reflectionDirection, viewDirection);
    float specular = pow(max(0.0, specularStrength), 64);

    vec4 lightingColor = (0.33f + 0.5f * diff + specular) * vec4(uLightColor, 1);

    FragColor = lightingColor * uColor;
}
";
    }
}