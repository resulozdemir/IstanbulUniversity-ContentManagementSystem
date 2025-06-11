export interface Site {
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
  createdDate: string | null;
  modifiedDate: string | null;
  pageCount: number;
  newsCount: number;
  eventCount: number;
  componentCount: number;
  domains: any[];
}
