/**
 * Theme - /api/Themes ve /api/Themes/{id} endpoint'lerinden gelen veri
 */
export interface Theme {
  id: number;
  name: string;
  isDeleted: boolean;
  // Component'te kullanılan ekstra özellikler
  primaryColor?: string;
  secondaryColor?: string;
  description?: string;
  footer?: string;
}
