/**
 * Base Component - /api/Components/{id} endpoint'inden gelen veri
 */
export interface Component {
  id: number;
  name: string;
  description: string;
  template: string;
  style: string;
  script: string;
  formJson?: string;
  componentType: number;
  isActive: boolean;
}

/**
 * Theme Component - /api/Components/themecomponent/{id} endpoint'inden gelen veri
 */
export interface ThemeComponent {
  id: number;
  themeId: number;
  componentId: number;
  name: string;
  description: string;
  template: string;
  style: string;
  javascript: string;
  formJson: string;
  formHtml: string | null;
  formJs: string | null;
  isDeleted: number;
}

/**
 * Site Component - /api/Components/forsite/{siteId} endpoint'inden gelen veri
 */
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

/**
 * Site Component Data - /api/Components/sitedata/{id} endpoint'inden gelen veri
 */
export interface SiteComponentData {
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

/**
 * Component data'sının parse edilmiş hali
 */
export interface ParsedComponentData {
  [key: string]: any;
}

/**
 * Component form alanları
 */
export interface ComponentFormField {
  name: string;
  label: string;
  type: string;
  fields?: ComponentFormField[];
}

/**
 * Component form JSON yapısı
 */
export interface ComponentForm {
  fields: ComponentFormField[];
}
