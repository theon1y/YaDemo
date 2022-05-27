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
in vec3 vNormal;
in vec4 vBoneWeights;
in vec4 vBoneIds;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

uniform vec3 lightColor;
uniform vec3 lightDirection;

const int MAX_BONES = 64;
const int MAX_NESTING = 4;
uniform mat4 uFinalBoneMatrices[MAX_BONES];

out vec2 fUv;
out vec3 fNormal;
out vec4 fDiffuse;

void main()
{
    mat4 boneTransform = mat4(0.0f);
    for(int i = 0 ; i < MAX_NESTING ; i++)
    {
        int id = int(vBoneIds[i]);
        boneTransform += uFinalBoneMatrices[id] * vBoneWeights[i];
    }

    vec4 totalPosition = boneTransform * vec4(vPos,1.0f);
    gl_Position = uProjection * uView * uModel * totalPosition;

    vec3 totalNormal = mat3(boneTransform) * vNormal;
    vec3 normal = normalize(mat3(transpose(inverse(uModel))) * totalNormal);
    float diff = max(dot(normal, normalize(lightDirection)), 0.0);
    fDiffuse = vec4(diff * lightColor, 1.0);

    fUv = vUv;
}
";

        private const string Fragment = @"
#version 330 core

in vec2 fUv;
in vec3 fNormal;
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