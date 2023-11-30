#version 420 core

#define ComplexMul(a, b) vec2(a.x*b.x-a.y*b.y, a.x*b.y+a.y*b.x)
#define ComplexDiv(a, b) vec2(((a.x*b.x+a.y*b.y)/(b.x*b.x+b.y*b.y)),((a.y*b.x-a.x*b.y)/(b.x*b.x+b.y*b.y)))
#define ComplexSin(a) vec2(sin(a.x) * cosh(a.y), cos(a.x) * sinh(a.y))
#define ComplexCos(a) vec2(cos(a.x) * cosh(a.y), -sin(a.x) * sinh(a.y))

uniform int maxIterations;
uniform vec3 color;
uniform float intensity;
uniform vec2 delta;
uniform float scale;
uniform vec2 resolution;
uniform float powing;
uniform vec2 constant; 
uniform int isMandelbrot;

vec2 ComplexTan(vec2 a) {return ComplexDiv(ComplexSin(a), ComplexCos(a)); }
vec2 ComplexCtan(vec2 a) {return ComplexDiv(ComplexCos(a), ComplexSin(a)); }


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

vec4 GetColorIRacle(int it) {
    float newValue = (float(it) / float(maxIterations) * intensity);

    vec2 st = gl_FragCoord.xy / resolution.xy;

    vec3 color1 = vec3(219, 244, 255) / 256.0;
    vec3 color2 = vec3(47, 92, 255) / 256.0;
    
    float mixValue = distance(st,vec2(1,0));
    vec3 colorMod = mix(color1,color2,mixValue);

    vec3 newColor = mix(colorMod, color, 0.5) * newValue;
    return vec4(newColor, 1.0f);
}

vec3 GetColorNew(float it) {
    vec3 a = vec3(0.0, 0.5, 0.5);
    vec3 b = vec3(0.0, 0.5, 0.5);
    vec3 c = vec3(0.0, 0.5, 0.33);
    vec3 d = vec3(0.0, 0.5, 0.66);

    return a + b * cos(6.28318 * (c * it + d));
}


void main()
{
  vec2 uv = GetCoord(gl_FragCoord.xy, resolution.yy);
  vec2 z = uv;
  vec2 c = constant;
  if (isMandelbrot == 1) {
    c += uv;
  }
  int it = 0;
  for (int i = 0; i < maxIterations; i++) {
    z = ComplexPow(z, powing) + c;
    if (length(z) >= 2.0f) {
      break;
    }
    it++;
  }
  if (it >= maxIterations) {
    gl_FragColor = vec4(0.0f, 0.0f, 0.0f, 1.0f);
    return;
  }
  else {
    gl_FragColor = GetColorIRacle(it);
  }
}