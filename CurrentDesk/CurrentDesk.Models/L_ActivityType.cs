
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


namespace CurrentDesk.Models
{

public partial class L_ActivityType
{
    #region Primitive Properties
    

    public virtual int PK_ActivityTypeID
    {

        get;
        set;

    }


    public virtual string ActivityTypeValue
    {

        get;
        set;

    }

        #endregion

        #region Navigation Properties
    


    public virtual ICollection<UserActivity> UserActivities
    {
        get
        {
            if (_userActivities == null)
            {

                var newCollection = new FixupCollection<UserActivity>();
                newCollection.CollectionChanged += FixupUserActivities;
                _userActivities = newCollection;

            }
            return _userActivities;
        }
        set
        {

            if (!ReferenceEquals(_userActivities, value))
            {
                var previousValue = _userActivities as FixupCollection<UserActivity>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupUserActivities;
                }
                _userActivities = value;
                var newValue = value as FixupCollection<UserActivity>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupUserActivities;
                }
            }

        }
    }
    private ICollection<UserActivity> _userActivities;

        #endregion

        #region Association Fixup
    

    private void FixupUserActivities(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (UserActivity item in e.NewItems)
            {

                item.L_ActivityType = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (UserActivity item in e.OldItems)
            {

                if (ReferenceEquals(item.L_ActivityType, this))
                {
                    item.L_ActivityType = null;
                }

            }
        }
    }

        #endregion

    
}

}
