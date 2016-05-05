title: Lists
permalink: /knowledgebase/lists/
order: 4
---

## Lists (mapping collections of nodes)

Ditto has the ability to map collections of any item that is stored as `IPublishedContent` within the published content cache. This is done my means of the built in [TypeConverters](usage-advanced-typeconverters/) that are included within the package.

TypeConverter attributes can be added to properties or classes and will instruct Ditto to automatically map to your POCO collection. 

If you are working with single document types then you can automatically map your POCO's to that type as follows.

    [TypeConverter(typeof(DittoPickerConverter))]
    public MyChildClass{
    }
    
    [TypeConverter(typeof(DittoPickerConverter))]
    public MyParentClass{
    
        // It is recommended to use lazy loading when referencing other IPublishedContent instances.
        public virtual IEnumerable<MyChildClass> Children {get; set;}
    }

If you are referencing multiple types you might want to list the property as follows.

    [TypeConverter(typeof(DittoPickerConverter))]
    public MyParentClass{
    
        // It is recommended to use lazy loading when referencing other IPublishedContent instances.
        [TypeConverter(typeof(DittoPickerConverter))]
        public virtual IEnumerable<IPublishedContent> Children {get; set;}
    }

You can then filter the items in the collection by document typ alias and convert them using individual calls to `As<T>`. 
