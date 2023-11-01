shader_type spatial;

uniform sampler2D render_texture;


void fragment() {
	ALBEDO = texture(render_texture, SCREEN_UV).xyz;
}

void light(){
	 DIFFUSE_LIGHT = vec3(0.7);
	
}