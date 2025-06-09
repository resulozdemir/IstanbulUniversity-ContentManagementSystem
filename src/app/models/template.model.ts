/**
 * Template - /api/Templates ve /api/Templates/{id} endpoint'lerinden gelen veri
 */
export interface Template {
  id: number;
  name: string;
  themeId: number;
  themeName: string | null;
  language: string;
  createdDate: string;
  modifiedDate: string | null;
  description?: string;
}

/**
 * Template'den türetilen site - /api/Templates/{id}/sites endpoint'inden gelen veri
 */
export interface TemplateSite {
  id: number;
  name: string;
  domain: string | null;
  templateId: number;
  templateName: string | null;
  themeId: number;
  themeName: string | null;
  language: string;
  istemplate: number;
  ispublish: number;
  createdDate: string;
  modifiedDate: string | null;
  pageCount: number;
  newsCount: number;
  eventCount: number;
  componentCount: number;
  domains: any[];
}

export interface TemplateTheme {
  id: number;
  templateId: number;
  themeId: number;
  name: string;
  description: string;
  createdDate: string;
  createdUser: string;
  modifiedDate: string;
  modifiedUser: string;
  isDeleted: boolean;
}

export interface Site {
  id: number;
  domain: string;
  subdomain: string;
  templateThemeId: number;
  name: string;
  description: string;
  createdDate: string;
  createdUser: string;
  modifiedDate: string;
  modifiedUser: string;
  isDeleted: boolean;
}

export interface SiteComponent {
  id: number;
  siteId: number;
  themeComponentId: number;
  componentId: number | null;
  componentName: string;
  data: string;
  column1: string | null;
  column2: string | null;
  column3: string | null;
  column4: string | null;
  createdDate: string | null;
  createdUser: string | null;
  modifiedDate: string | null;
  modifiedUser: string | null;
  isDeleted: number;
  dataJson: string;
  settings: string;
  pageId: number;
  orderBy: number;
  isActive: boolean;
}

export interface SiteComponentData {
  id: number;
  siteId: number;
  themeComponentId: number;
  data: string;
  createdDate: string;
  createdUser: string;
  modifiedDate: string;
  modifiedUser: string;
  isDeleted: boolean;
}
