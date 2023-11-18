shader_type canvas_item;

uniform float isHighlighted;
const vec4 BRIGHT = vec4(0.4, 0.4, 0.4, 1);
const vec4 DARK = vec4(0.1, 0.1, 0.1, 1);

float gradient(float x)
{
	return pow( 6.0*x*(1.0-x), 1 );
}

void fragment() {
	vec4 col = mix(DARK, BRIGHT, step(0.5f, isHighlighted));
	
	col.a = gradient(UV.y);
	
	
	COLOR = col;
}
