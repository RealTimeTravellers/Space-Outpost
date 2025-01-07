shader_type spatial;

uniform vec4 hologram_color : source_color = vec4(0.0, 1.0, 0.8, 0.5);
uniform float scan_line_speed = 2.0;
uniform float scan_line_size = 4.0;
uniform float flicker_intensity = 0.1;
uniform float transparency = 0.5;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform float transition : hint_range(0.0, 1.0) = 0.0;

void fragment() {
    // Ana materyal rengi
    vec4 base_color = texture(SCREEN_TEXTURE, SCREEN_UV);
    
    // Tarama çizgileri
    float scan_line = sin(UV.y * scan_line_size + TIME * scan_line_speed) * 0.1;
    
    // Titreşim efekti
    float flicker = sin(TIME * 10.0) * flicker_intensity;
    
    // Hologram rengi
    vec4 final_color = hologram_color;
    final_color.a *= (0.8 + scan_line + flicker) * transparency;
    
    // Geçiş efekti
    ALBEDO = mix(base_color.rgb, final_color.rgb, transition);
    ALPHA = mix(1.0, final_color.a, transition);
    
    // Fresnel efekti
    float fresnel = pow(1.0 - dot(NORMAL, VIEW), 2.0);
    EMISSION = hologram_color.rgb * fresnel * transition;
}