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

out vec3 fPos;
out vec2 fUv;
out vec3 fNormal;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(vPos, 1);
    fPos = vec3(uModel * vec4(vPos, 1));
    fUv = vUv;
    fNormal = normalize(mat3(transpose(inverse(uModel))) * vNormal);
}
";

        private const string Fragment = @"
#version 330 core

in vec2 fUv;
in vec3 fPos;
in vec3 fNormal;

uniform sampler2D uTexture0;
uniform vec4 uTexture0Uv;

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

    vec2 albedoUv = fUv * uTexture0Uv.xy + uTexture0Uv.zw;
    vec4 albedoColor = texture(uTexture0, albedoUv);
    FragColor = lightingColor * albedoColor;
}
";
    }
}