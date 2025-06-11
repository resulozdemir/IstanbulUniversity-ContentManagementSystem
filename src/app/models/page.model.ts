export interface Page {
  id: number;
  name: string;
  siteId: number;
  siteName: string;
  parentId: number;
  parentName: string;
  routing: string;
  createdDate: string;
  isDeleted: number;
  showInMenu: boolean;
  menuOrder: number;
  childCount: number;
}
