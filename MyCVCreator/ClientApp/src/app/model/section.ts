interface SectionItem {
  id: number,
  type: sectionType,
}

interface ListItem {
  id: number,
  text: string
}

export interface BulletSectionItem extends SectionItem {
  list: ListItem[],
  type: "bullet",
}

export interface TextSectionItem extends SectionItem {
  text: string,
  type: "text",
}

export interface LabeledSectionItem extends SectionItem {
  label: string,
  text: string,
  type: "labeled",
}

type SectionItemTypes = LabeledSectionItem | BulletSectionItem | TextSectionItem; 

export interface Section {
  id: number,
  title: string,
  items: SectionItemTypes[]
}

export type sectionType = "bullet" | "text" | "labeled";

export function createSectionItem(itemType: sectionType, id: number): SectionItemTypes {
  switch (itemType) {
    case "bullet":
      const bulletItem: BulletSectionItem = {
        id: id,
        list: [],
        type: itemType
      };

      return bulletItem;

    case "labeled":
      const labeledItem: LabeledSectionItem = {
        id: id,
        label: '',
        text: '',
        type: itemType
      };

      return labeledItem;

    case "text":
      const textItem: TextSectionItem = {
        id: id,
        text: '',
        type: itemType
      };

      return textItem;
  }
}
