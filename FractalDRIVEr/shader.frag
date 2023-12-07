#version 330 core

#define ComplexMul(a, b) vec2(a.x*b.x-a.y*b.y, a.x*b.y+a.y*b.x)
#define ComplexDiv(a, b) vec2(((a.x*b.x+a.y*b.y)/(b.x*b.x+b.y*b.y)),((a.y*b.x-a.x*b.y)/(b.x*b.x+b.y*b.y)))
#define ComplexSin(a) vec2(sin(a.x) * cosh(a.y), cos(a.x) * sinh(a.y))
#define ComplexCos(a) vec2(cos(a.x) * cosh(a.y), -sin(a.x) * sinh(a.y))

#define PI 3.1415926535897932384626433832795

uniform int maxIterations;
uniform vec3 color;
uniform float intensity;
uniform vec2 delta;
uniform float scale;
uniform vec2 resolution;
uniform float powing;
uniform vec2 constant; 
uniform int fractType;
uniform int functionType;
uniform int coloring;
bool smoothIterations = true;

vec2 ComplexTan(vec2 a) 
{
    return ComplexDiv(ComplexSin(a), ComplexCos(a)); 
}

vec2 ComplexCtan(vec2 a) 
{
    return ComplexDiv(ComplexCos(a), ComplexSin(a)); 
}

vec2 ComplexLog(vec2 a) {
    float rpart = sqrt((a.x*a.x)+(a.y*a.y));
    float ipart = atan(a.y,a.x);
    if (ipart > PI) 
        ipart = ipart - (2.0 * PI);
    return vec2(log(rpart),ipart);
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

vec2 ComplexPow(vec2 a, float n) {
    float angle = atan(a.y, a.x);
    float r = length(a);
    float real = pow(r, n) * cos(n * angle);
    float im = pow(r, n) * sin(n * angle);
    return vec2(real, im);
}

vec2 GetCoord(vec2 cord, vec2 res)
{
  return (delta - (scale / vec2(2.0f, 2.0f))) + cord * (scale / res);
}

vec4 GetColorIRacle(int it, bool isNew) {
    float newValue = float(it) / float(maxIterations) * intensity;
    if (isNew) {
        newValue = fract(newValue + 0.5);
    }

    vec2 st = gl_FragCoord.xy / resolution.xy;

    vec3 color1 = vec3(219, 244, 255) / 256.0;
    vec3 color2 = vec3(47, 92, 255) / 256.0;
    
    float mixValue = distance(st,vec2(1,0));
    vec3 colorMod = mix(color1,color2,mixValue);

    vec3 newColor = mix(colorMod, color, 0.5) * newValue;
    return vec4(newColor, 1.0f);
}

vec3 GetColorGlobal(float it, bool isNew) {
    float val = float(it) / maxIterations * intensity;
    if (!isNew) {
        val = fract(val + 0.5);
    }
    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.6, 0.5, 0.5);
    vec3 c = vec3(1.0, 1.0, 1.0);
    vec3 d = vec3(0.0, 0.1, 0.2);

    return a + b * cos(6.28318 * (c * val + d));
}

void main()
{
  vec2 uv = GetCoord(gl_FragCoord.xy, resolution.yy);
  vec2 z = uv;
  vec2 c = constant;
  if (fractType != 1) {
    c += uv;
  }
  int it = 0;
  for (int i = 0; i < maxIterations; i++) {
    if (fractType == 2) {
        z = vec2(abs(z.x), abs(z.y));
    }
    switch(functionType) {
        case 0:
            z = ComplexPow(z, powing);
            break;
        case 1:
            z = ComplexSin(ComplexPow(z, powing));
            break;
        case 2:
            z = ComplexCos(ComplexPow(z, powing));
            break;
        case 3:
            z = ComplexTan(ComplexPow(z, powing));
            break;
        case 4:
            z = ComplexCtan(ComplexPow(z, powing));
            break;
        case 5:
            z = ComplexSinh(ComplexPow(z, powing));
            break;
        case 6:
            z = ComplexCosh(ComplexPow(z, powing));
            break;
        case 7:
            z = ComplexTanh(ComplexPow(z, powing));
            break;
        case 8:
            z = ComplexCtanh(ComplexPow(z, powing));
            break;
    }
    z += c;
    if (dot(z, z) > 4.0f) {
      break;
    }
    it++;
  }
  if (it >= maxIterations) {
    gl_FragColor = vec4(0.0f, 0.0f, 0.0f, 1.0f);
    return;
  }
  else {
    if (coloring % 2 == 0) {
        gl_FragColor = GetColorIRacle(it, (coloring >> 1) % 2 != 0);
    }
    else {
        gl_FragColor = vec4(GetColorGlobal(it, (coloring >> 1) % 2 != 0), 1);
    }
  }
}