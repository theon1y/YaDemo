using YaEngine.Render;

namespace YaDemo
{
    public class DiffuseAnimationShader
    {
        public static readonly ShaderInitializer Value = new(nameof(DiffuseAnimationShader),
            new StringShaderProvider(Vertex),
            new StringShaderProvider(Fragment));
        
        private const string Vertex = @"
#version 330 core
in vec3 vPos;
in vec2 vUv;
in vec3 vTangent;
in vec3 vBitangent;
in vec3 vNormal;
in vec4 vBoneWeights;
in vec4 vBoneIds;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

uniform vec3 uViewPosition;
uniform vec3 uLightPosition;

const int MAX_BONES = 64;
const int MAX_NESTING = 4;

uniform mat4 uFinalBoneMatrices[MAX_BONES];

out vec2 fUv;
out vec3 fPosition;
out vec3 fLightPosition;
out vec3 fViewPosition;

void main()
{
    mat4 boneTransform = mat4(0);
    for(int i = 0 ; i < MAX_NESTING; ++i)
    {
        int id = int(vBoneIds[i]);
        boneTransform += uFinalBoneMatrices[id] * vBoneWeights[i];
    }

    vec4 totalPosition = boneTransform * vec4(vPos, 1);
    gl_Position = uProjection * uView * uModel * totalPosition;

    mat3 boneTransform3 = mat3(boneTransform);
    vec3 normal = normalize(boneTransform3 * vNormal);
    vec3 tangent = normalize(boneTransform3 * vTangent);
    vec3 bitangent = normalize(boneTransform3 * vBitangent);
    mat3 tbn = transpose(mat3(tangent, bitangent, normal));

    fLightPosition = tbn * uLightPosition;
    fViewPosition = tbn * uViewPosition;
    fPosition = tbn * vec3(uModel * totalPosition);

    fUv = vUv;
}
";

        private const string Fragment = @"
#version 330 core

in vec2 fUv;
in vec3 fPosition;
in vec3 fLightPosition;
in vec3 fViewPosition;

uniform vec3 uLightColor;

uniform sampler2D uTexture0;
uniform sampler2D uNormalTexture0;
uniform sampler2D uSpecularTexture0;

uniform float uAmbientStrength;
uniform float uDiffuseStrength;
uniform float uSpecularStrength;
uniform float uShiness;

out vec4 FragColor;

void main()
{
    vec4 albedoColor = texture(uTexture0, fUv);

    vec3 ambientColor = uAmbientStrength * albedoColor.rgb;

    vec3 normal = texture(uNormalTexture0, fUv).rgb;
    normal = normalize(normal * 2 - 1);
    vec3 lightDirection = normalize(fLightPosition - fPosition);
    float diff = max(dot(lightDirection, normal), 0);
    vec3 diffuseColor = uDiffuseStrength * diff * albedoColor.rgb;

    vec3 viewDirection = normalize(fViewPosition - fPosition);
    vec3 reflectionDirection = reflect(-lightDirection, normal);
    float specularStrength = dot(reflectionDirection, viewDirection);
    float specular = pow(max(0.0, specularStrength), uShiness);
    vec3 specularColor = uSpecularStrength * specular * texture(uSpecularTexture0, fUv).rgb;

    FragColor = vec4(ambientColor + diffuseColor + specularColor, 1);
}
";
    }
}