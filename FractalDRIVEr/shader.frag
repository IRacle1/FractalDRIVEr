#version 440 core

#define ComplexMul(a, b) vec2(a.x*b.x-a.y*b.y, a.x*b.y+a.y*b.x)
#define ComplexDiv(a, b) vec2(((a.x*b.x+a.y*b.y)/(b.x*b.x+b.y*b.y)),((a.y*b.x-a.x*b.y)/(b.x*b.x+b.y*b.y)))
#define ComplexSin(a) vec2(sin(a.x) * cosh(a.y), cos(a.x) * sinh(a.y))
#define ComplexCos(a) vec2(cos(a.x) * cosh(a.y), -sin(a.x) * sinh(a.y))

#define PI 3.1415926535897932384626433832795

uniform int MaxIterations;
uniform float Intensity;
uniform vec2 Delta;
uniform float Scale;
uniform vec2 resolution;
uniform vec4 Color;
uniform int SmoothMode;
uniform float Barier;

// 0 - FractType
// 1 - MainFunctionType
// 2 - BeforeFunctionType
// 3 - ConstantFlag

uniform int[4] OldBehaviour;
uniform int[4] Behaviour;

// 0-1 - Pow
// 2-3 - Const
uniform float[4] OldVariables;
uniform float[4] Variables;

uniform float PeriodPersent;

uniform float Time;
uniform int Pixel;

uniform int InversedColor;

out vec4 FragColor;

mat4x3 Global = mat4x3(
    vec3(0.0, 0.0, 0.0), 
    vec3(0.6, 0.5, 0.5),
    vec3(1.0, 1.0, 1.0),
    vec3(0.0, 0.1, 0.2));

float random(in vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.989, 78.233))) * 43758.543);
}

float rseed = 0.;

vec2 random2() {
    vec2 seed = vec2(rseed++, rseed++);
    return vec2(random(seed + 0.342), random(seed + 0.756));
}

vec2 ComplexTan(vec2 a) {
    return ComplexDiv(ComplexSin(a), ComplexCos(a));
}

vec2 ComplexCtan(vec2 a) {
    return ComplexDiv(ComplexCos(a), ComplexSin(a));
}

vec2 ComplexLn(vec2 a) {
    float rpart = length(a);
    float ipart = atan(a.y, a.x);
    if(ipart > PI)
        ipart = ipart - (2.0 * PI);
    return vec2(log(rpart), ipart);
}

vec2 ComplexExp(vec2 a) {
    return exp(a.x) * vec2(cos(a.y), sin(a.y));
}

vec2 ComplexCosh(vec2 z) {
    return (ComplexExp(z) + ComplexExp(-z)) / 2.0;
}

vec2 ComplexSinh(vec2 z) {
    return (ComplexExp(z) - ComplexExp(-z)) / 2.0;
}

vec2 ComplexTanh(vec2 z) {
    return ComplexDiv(ComplexSinh(z), ComplexCosh(z));
}

vec2 ComplexCtanh(vec2 z) {
    return ComplexDiv(ComplexCosh(z), ComplexSinh(z));
}

vec2 ComplexPowFull(vec2 a, vec2 b) {
    float angle = atan(a.y, a.x);
    float r = length(a);
    float newAngle = b.x * angle + b.y * log(r);
    float newR = pow(r, b.x) * exp(-b.y * angle);
    return newR * vec2(cos(newAngle), sin(newAngle));
}

vec2 GetCoord(vec2 cord, vec2 res) {
    return (Delta - (Scale / vec2(2.0, 2.0))) + cord * (Scale / res);
}

vec2 GetCoordRand(vec2 cord, vec2 res) {
    return (Delta - (Scale / vec2(2.0, 2.0))) + (cord + random2()) * (Scale / res);
}

vec4 GetColorGlobal(float it, mat4x3 pattern, vec3 col) {
    float val = it / MaxIterations * Intensity;

    val = fract(val + 0.5);

    vec3 newColor = vec3(col + pattern[1] * cos(6.28318 * (pattern[2] * val + pattern[3])));

    if (InversedColor == 1) {
        newColor = vec3(1.0f) - newColor;
    }

    return vec4(newColor, 1.0);
}

vec2 DoFunction(int num, vec2 z) {
    vec2 ret = z;
    switch(num) {
        case 1:
            ret = ComplexSin(z);
            break;
        case 2:
            ret = ComplexCos(z);
            break;
        case 3:
            ret = ComplexTan(z);
            break;
        case 4:
            ret = ComplexCtan(z);
            break;
        case 5:
            ret = ComplexSinh(z);
            break;
        case 6:
            ret = ComplexCosh(z);
            break;
        case 7:
            ret = ComplexTanh(z);
            break;
        case 8:
            ret = ComplexCtanh(z);
            break;
        case 9:
            ret = ComplexLn(z);
            break;
        case 10:
            ret = vec2(z.x, -z.y);
            break;
        case 11:
            ret = ComplexDiv(vec2(1, 0), z);
            break;
        case 12:
            ret = abs(z);
            break;
    }

    return ret;
}

bool ExitAlgorithm(float it, vec2 z, float barier) {
    return Barier > 0 ? dot(z, z) > Barier * Barier : dot(z, z) < Barier * Barier;
}

vec2 ExecFunction(vec2 z, vec2 pow, vec2 c, int[4] behaviour) {
    vec2 newZ = z;
    newZ = DoFunction(behaviour[2], newZ);
    newZ = DoFunction(behaviour[1], ComplexPowFull(newZ, pow));

    switch (behaviour[3]) {
        case 0:
            newZ += c;
            break;
        case 1:
            newZ = ComplexPowFull(newZ, c);
            break;
        case 2:
            newZ = ComplexPowFull(c, newZ);
            break;
    }

    return newZ;
}

float MainCalculate(vec2 uv, int[4] behaviour, float[4] variables) {
    vec2 z = uv;
    vec2 c = vec2(variables[2], variables[3]);
    vec2 pow = vec2(variables[0], variables[1]);
    if(behaviour[0] != 1) {
        c += uv;
    }

    int it = 0;

    for(int i = 0; i < MaxIterations; i++) {
        z = ExecFunction(z, pow, c, behaviour);

        if (ExitAlgorithm(it, z, Barier)) {
            break;
        }

        it++;
    }

    return it;
}

float SmartCalculate(vec2 uv, int[4] behaviourOne, int[4] behaviourTwo, float[4] variablesOne, float[4] variablesTwo, float coef) {
    vec2 z = uv;
    vec2 c = mix(vec2(variablesOne[2], variablesOne[3]), vec2(variablesTwo[2], variablesTwo[3]), coef);
    vec2 pow = mix(vec2(variablesOne[0], variablesOne[1]), vec2(variablesTwo[0], variablesTwo[1]), coef);

    if (behaviourOne[0] == 0 && behaviourTwo[0] == 0) {
        c += uv;
    }
    else if (behaviourOne[0] == 0 && behaviourTwo[0] == 1) {
        c += uv * (1 - coef);
    }
    else if (behaviourOne[0] == 1 && behaviourTwo[0] == 0) {
        c += uv * coef;
    }

    int it = 0;

    for(int i = 0; i < MaxIterations; i++) {
        vec2 first = ExecFunction(z, pow, c, behaviourOne);
        vec2 second = ExecFunction(z, pow, c, behaviourTwo);
        
        z = mix(first, second, coef);

        if (ExitAlgorithm(it, z, Barier)) {
            break;
        }

        it++;
    }

    return it;
}

vec4 PostCalculate(float it) {
    if(it >= MaxIterations) {
        return vec4(0.0);
    }
    else {
        float newIt = float(it);
        return GetColorGlobal(newIt, Global, Color.xyz);
    }
}

void main() {
    vec2 coord = vec2(int(gl_FragCoord.x / Pixel), int(gl_FragCoord.y / Pixel));
    vec2 res = vec2(int(resolution.y / Pixel));

    if(SmoothMode == 0) {

        vec2 uv = GetCoord(coord, res);

        if (PeriodPersent == 0.0f) {
            FragColor = PostCalculate(MainCalculate(uv, Behaviour, Variables));
        }
        else if (PeriodPersent == 1.0f) {
            FragColor = PostCalculate(MainCalculate(uv, OldBehaviour, OldVariables));
        }
        else {
            FragColor = PostCalculate(SmartCalculate(uv, Behaviour, OldBehaviour, Variables, OldVariables, PeriodPersent));
        }
        return;
    }

    vec3 col = vec3(0.0);
    for(int i = 0; i < 4; i++) {
        vec2 uv = GetCoordRand(coord, res);
        vec4 temp = PostCalculate(MainCalculate(uv, Behaviour, Variables));
        if (temp == vec4(0.0)) {
            FragColor = vec4(temp.xyz, 1.0);
            return;
        }
        col += temp.xyz;
    }
    FragColor = vec4(col / 4, 1.0);
}