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
uniform vec2 Pow;
uniform vec2 Constant;
uniform int ConstantFlag;
uniform int FractType;
uniform int MainFunctionType;
uniform int BeforeFunctionType;
uniform int ColoringType;
uniform int SmoothMode;
uniform float Barier;

out vec4 FragColor;

mat4x3 color1 = mat4x3(
    vec3(0.5, 0.5, 0.5), 
    vec3(0.6, 0.5, 0.5),
    vec3(1.0, 1.0, 1.0),
    vec3(0.0, 0.1, 0.2));

mat4x3 color2 = mat4x3(
    vec3(0.5, 0.5, 0.5), 
    vec3(0.6, 0.5, 0.5),
    vec3(2.0, 1.0, 0.0),
    vec3(0.5, 0.2, 0.25));

mat4x3 color3 = mat4x3(
    vec3(0.000, 0.500, 0.500), 
    vec3(0.000, 0.500, 0.500),
    vec3(0.000, 0.500, 0.333),
    vec3(0.000, 0.500, 0.667));

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

vec4 GetStableColor(vec3 color, float it, bool isNew) {
    float newValue = float(it) / float(MaxIterations) * Intensity;

    if(isNew) {
        newValue = fract(newValue + 0.5);
    }

    vec3 newColor = color * newValue;
    return vec4(newColor, 1.0);
}

vec4 GetColorGlobal(float it, mat4x3 pattern, bool isNew) {
    float val = float(it) / MaxIterations * Intensity;
    if(!isNew) {
        val = fract(val + 0.5);
    }

    return vec4(pattern[0] + pattern[1] * cos(6.28318 * (pattern[2] * val + pattern[3])), 1.0);
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
            ret = abs(z);
    }

    return ret;
}

vec4 mainCalculate(vec2 uv) {
    vec2 z = uv;
    vec2 c = Constant;
    if(FractType != 1) {
        c += uv;
    }
    int it = 0;
    for(int i = 0; i < MaxIterations; i++) {
        z = DoFunction(BeforeFunctionType, z);
        z = DoFunction(MainFunctionType, ComplexPowFull(z, Pow));
        
        switch (ConstantFlag) {
            case 0:
                z += c;
                break;
            case 1:
                z = ComplexPowFull(z, c);
                break;
            case 2:
                z = ComplexPowFull(c, z);
                break;
        }

        if(Barier > 0 ? dot(z, z) > Barier * Barier : dot(z, z) < Barier * Barier) {
            break;
        }

        it++;
    }
    if(it >= MaxIterations) {
        return vec4(0.0, 0.0, 0.0, 0.0);
    } 
    else {
        float newIt = float(it);
        if(SmoothMode == 1) {
            newIt = it - log(log(dot(z, z)) / log(Barier)) / log(2.);
        }

        if(ColoringType == 0) {
            return GetStableColor(vec3(1.0), newIt, false);
        }
        if(ColoringType == 1) {
            return GetColorGlobal(newIt, color1, false);
        }
        if(ColoringType == 2) {
            return GetColorGlobal(newIt, color2, false);
        }
        if(ColoringType == 3) {
            return GetColorGlobal(newIt, color3, true);
        }
    }
}

void main() {
    if(SmoothMode == 0) {
        FragColor = mainCalculate(GetCoord(gl_FragCoord.xy, resolution.yy));
        return;
    }

    vec3 col = vec3(0.0);
    for(int i = 0; i < 4; i++) {
        vec2 uv = GetCoordRand(gl_FragCoord.xy, resolution.yy);
        vec4 temp = mainCalculate(uv);
        if (temp == vec4(0.0)) {
            FragColor = vec4(temp.xyz, 1.0);
            return;
        }
        col += temp.xyz;
    }
    FragColor = vec4(col / 4, 1.0);
}