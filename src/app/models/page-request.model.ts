export interface PageRequest {
  id: number;
  name: string;
  html: string;
  siteId: number;
  parentId: number;
  routing: string;
  metaTitle: string;
  metaDescription: string;
  metaKeywords: string;
  isDeleted: number;
  showInMenu: boolean;
  menuOrder: number;
  layout: string;
  style: string;
  javascript: string;
}
