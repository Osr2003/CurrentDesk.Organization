
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

public partial class Transaction
{
    #region Primitive Properties
    

    public virtual int PK_TransactionID
    {

        get;
        set;

    }


    public virtual Nullable<int> FK_FromCurrencyID
    {

        get;
        set;

    }


    public virtual Nullable<int> FK_ToCurrencyID
    {

        get;
        set;

    }


    public virtual Nullable<decimal> Amount
    {

        get;
        set;

    }


    public virtual string TransactionType
    {

        get;
        set;

    }


    public virtual string ExchangeRate
    {

        get;
        set;

    }


    public virtual string Notes
    {

        get;
        set;

    }


    public virtual string FromAccount
    {

        get;
        set;

    }


    public virtual string ToAccount
    {

        get;
        set;

    }


    public virtual Nullable<System.DateTime> TransactionDateTime
    {

        get;
        set;

    }


    public virtual int FK_OrganizationID
    {

        get { return _fK_OrganizationID; }
        set
        {

            if (_fK_OrganizationID != value)

            {

                if (Organization != null && Organization.PK_OrganizationID != value)

                {

                    Organization = null;

                }

                _fK_OrganizationID = value;
            }

        }

    }

    private int _fK_OrganizationID;

        #endregion

        #region Navigation Properties
    


    public virtual ICollection<TransferLog> TransferLogs
    {
        get
        {
            if (_transferLogs == null)
            {

                var newCollection = new FixupCollection<TransferLog>();
                newCollection.CollectionChanged += FixupTransferLogs;
                _transferLogs = newCollection;

            }
            return _transferLogs;
        }
        set
        {

            if (!ReferenceEquals(_transferLogs, value))
            {
                var previousValue = _transferLogs as FixupCollection<TransferLog>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupTransferLogs;
                }
                _transferLogs = value;
                var newValue = value as FixupCollection<TransferLog>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupTransferLogs;
                }
            }

        }
    }
    private ICollection<TransferLog> _transferLogs;



    public virtual Organization Organization
    {

        get { return _organization; }
        set
        {
            if (!ReferenceEquals(_organization, value))
            {
                var previousValue = _organization;
                _organization = value;
                FixupOrganization(previousValue);
            }
        }
    }
    private Organization _organization;

        #endregion

        #region Association Fixup
    

    private void FixupOrganization(Organization previousValue)
    {

        if (previousValue != null && previousValue.Transactions.Contains(this))
        {
            previousValue.Transactions.Remove(this);
        }


        if (Organization != null)
        {
            if (!Organization.Transactions.Contains(this))
            {
                Organization.Transactions.Add(this);
            }

            if (FK_OrganizationID != Organization.PK_OrganizationID)

            {
                FK_OrganizationID = Organization.PK_OrganizationID;
            }

        }

    }


    private void FixupTransferLogs(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (TransferLog item in e.NewItems)
            {

                item.Transaction = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (TransferLog item in e.OldItems)
            {

                if (ReferenceEquals(item.Transaction, this))
                {
                    item.Transaction = null;
                }

            }
        }
    }

        #endregion

    
}

}
