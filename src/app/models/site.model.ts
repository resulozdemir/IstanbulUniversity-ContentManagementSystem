export interface Site {
  id: number;
  name: string;
  domain: string | null;
  templateId: number;
  themeId: number;
  templateName: string | null;
  themeName: string | null;
  language: string;
  istemplate: number | boolean;
  ispublish: number | boolean;
  isPublish?: number | boolean; // Alternative field name
  isTemplate?: number | boolean; // Alternative field name
  createdDate: string | null;
  modifiedDate: string | null;
  pageCount?: number;
  newsCount?: number;
  eventCount?: number;
  componentCount?: number;
  domains?: any[];
  pages?: any[];
  components?: any[];
  availableComponents?: any[];
  // API'den gelebilecek olası ek alanlar
  url?: string;
  description?: string;
  title?: string;
  favicon?: string;
  metaDescription?: string;
  metaKeywords?: string;
  logo?: string;
  createdUser?: string;
  modifiedUser?: string;
  isDeleted?: boolean;
  isActive?: boolean;
  settings?: string;
  analyticId?: string | null;
  googleSiteVerification?: string | null;
  createdUserName?: string | null;
  modifiedUserName?: string | null;
  [key: string]: any; // Diğer herhangi bir alan için
}

export interface SitePage {
  id: number;
  siteId: number;
  name: string;
  title: string;
  url: string;
  description: string;
  content: string;
  metaDescription: string;
  metaKeywords: string;
  layout: string;
  parentId: number | null;
  isHome: boolean;
  isMenu: boolean;
  orderBy: number;
  createdDate: string;
  modifiedDate: string;
  createdUser: string;
  modifiedUser: string;
  isDeleted: boolean;
  isActive: boolean;
}
