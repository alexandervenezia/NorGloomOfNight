shader_type canvas_item;

const vec4 BLACK = vec4(0.025, 0, 0.05, 1);

float gradient(float x)
{
	return pow( 6.0*x*(1.0-x), 1 );
}

void fragment() {
	vec4 col = BLACK;
	
	col.a = gradient(UV.y);
	
	col = mix(col, vec4(0.7), step(0.99, UV.x));
	col = mix(col, vec4(0.7), step(UV.x, 0.01));
	
	COLOR = col;
}
