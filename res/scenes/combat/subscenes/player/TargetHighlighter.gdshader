shader_type canvas_item;

uniform vec4 HUE;

float vert_gradient(float y)
{
	float col = mix(1.0, 0.0, smoothstep(0.8, 1.03, y));
	col *= y;
	return col;
}

float horizontal_gradient(float x)
{
	x = (abs(0.5-x));

	float col = smoothstep(0.0, 1.0, x+0.35);
	col = mix(col, 0.1, x*2.0);
	return col;
}

void fragment() {
	// Place fragment code here.
	vec4 color = HUE;
	//color.a = UV.y * (abs(0.5f-UV.x) + 0.25f);
	color.a = vert_gradient(UV.y);
	color.a *= horizontal_gradient(UV.x);
	color.a *= 3.0;
	
	COLOR = color;
}
