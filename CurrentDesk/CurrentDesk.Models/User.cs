
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

public partial class User
{
    #region Primitive Properties
    

    public virtual int PK_UserID
    {

        get;
        set;

    }


    public virtual string UserEmailID
    {

        get;
        set;

    }


    public virtual string Password
    {

        get;
        set;

    }


    public virtual Nullable<int> FK_UserTypeID
    {

        get { return _fK_UserTypeID; }
        set
        {

            try
            {
                _settingFK = true;

            if (_fK_UserTypeID != value)

            {

                if (L_AccountFormType != null && L_AccountFormType.PK_AccountFormID != value)

                {

                    L_AccountFormType = null;

                }

                _fK_UserTypeID = value;
            }

            }
            finally
            {
                _settingFK = false;
            }

        }

    }

    private Nullable<int> _fK_UserTypeID;


    public virtual int FK_OrganizationID
    {

        get { return _fK_OrganizationID; }
        set
        {

            try
            {
                _settingFK = true;

            if (_fK_OrganizationID != value)

            {

                if (Organization != null && Organization.PK_OrganizationID != value)

                {

                    Organization = null;

                }

                _fK_OrganizationID = value;
            }

            }
            finally
            {
                _settingFK = false;
            }

        }

    }

    private int _fK_OrganizationID;

        #endregion

        #region Navigation Properties
    


    public virtual ICollection<AssetManager> AssetManagers
    {
        get
        {
            if (_assetManagers == null)
            {

                var newCollection = new FixupCollection<AssetManager>();
                newCollection.CollectionChanged += FixupAssetManagers;
                _assetManagers = newCollection;

            }
            return _assetManagers;
        }
        set
        {

            if (!ReferenceEquals(_assetManagers, value))
            {
                var previousValue = _assetManagers as FixupCollection<AssetManager>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupAssetManagers;
                }
                _assetManagers = value;
                var newValue = value as FixupCollection<AssetManager>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupAssetManagers;
                }
            }

        }
    }
    private ICollection<AssetManager> _assetManagers;



    public virtual ICollection<Client> Clients
    {
        get
        {
            if (_clients == null)
            {

                var newCollection = new FixupCollection<Client>();
                newCollection.CollectionChanged += FixupClients;
                _clients = newCollection;

            }
            return _clients;
        }
        set
        {

            if (!ReferenceEquals(_clients, value))
            {
                var previousValue = _clients as FixupCollection<Client>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupClients;
                }
                _clients = value;
                var newValue = value as FixupCollection<Client>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupClients;
                }
            }

        }
    }
    private ICollection<Client> _clients;



    public virtual ICollection<IntroducingBroker> IntroducingBrokers
    {
        get
        {
            if (_introducingBrokers == null)
            {

                var newCollection = new FixupCollection<IntroducingBroker>();
                newCollection.CollectionChanged += FixupIntroducingBrokers;
                _introducingBrokers = newCollection;

            }
            return _introducingBrokers;
        }
        set
        {

            if (!ReferenceEquals(_introducingBrokers, value))
            {
                var previousValue = _introducingBrokers as FixupCollection<IntroducingBroker>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupIntroducingBrokers;
                }
                _introducingBrokers = value;
                var newValue = value as FixupCollection<IntroducingBroker>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupIntroducingBrokers;
                }
            }

        }
    }
    private ICollection<IntroducingBroker> _introducingBrokers;



    public virtual L_AccountFormType L_AccountFormType
    {

        get { return _l_AccountFormType; }
        set
        {
            if (!ReferenceEquals(_l_AccountFormType, value))
            {
                var previousValue = _l_AccountFormType;
                _l_AccountFormType = value;
                FixupL_AccountFormType(previousValue);
            }
        }
    }
    private L_AccountFormType _l_AccountFormType;



    public virtual ICollection<PartnerCommission> PartnerCommissions
    {
        get
        {
            if (_partnerCommissions == null)
            {

                var newCollection = new FixupCollection<PartnerCommission>();
                newCollection.CollectionChanged += FixupPartnerCommissions;
                _partnerCommissions = newCollection;

            }
            return _partnerCommissions;
        }
        set
        {

            if (!ReferenceEquals(_partnerCommissions, value))
            {
                var previousValue = _partnerCommissions as FixupCollection<PartnerCommission>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupPartnerCommissions;
                }
                _partnerCommissions = value;
                var newValue = value as FixupCollection<PartnerCommission>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupPartnerCommissions;
                }
            }

        }
    }
    private ICollection<PartnerCommission> _partnerCommissions;



    public virtual ICollection<UserImage> UserImages
    {
        get
        {
            if (_userImages == null)
            {

                var newCollection = new FixupCollection<UserImage>();
                newCollection.CollectionChanged += FixupUserImages;
                _userImages = newCollection;

            }
            return _userImages;
        }
        set
        {

            if (!ReferenceEquals(_userImages, value))
            {
                var previousValue = _userImages as FixupCollection<UserImage>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupUserImages;
                }
                _userImages = value;
                var newValue = value as FixupCollection<UserImage>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupUserImages;
                }
            }

        }
    }
    private ICollection<UserImage> _userImages;



    public virtual ICollection<UserDocument> UserDocuments
    {
        get
        {
            if (_userDocuments == null)
            {

                var newCollection = new FixupCollection<UserDocument>();
                newCollection.CollectionChanged += FixupUserDocuments;
                _userDocuments = newCollection;

            }
            return _userDocuments;
        }
        set
        {

            if (!ReferenceEquals(_userDocuments, value))
            {
                var previousValue = _userDocuments as FixupCollection<UserDocument>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupUserDocuments;
                }
                _userDocuments = value;
                var newValue = value as FixupCollection<UserDocument>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupUserDocuments;
                }
            }

        }
    }
    private ICollection<UserDocument> _userDocuments;



    public virtual ICollection<ManagedAccountProgram> ManagedAccountPrograms
    {
        get
        {
            if (_managedAccountPrograms == null)
            {

                var newCollection = new FixupCollection<ManagedAccountProgram>();
                newCollection.CollectionChanged += FixupManagedAccountPrograms;
                _managedAccountPrograms = newCollection;

            }
            return _managedAccountPrograms;
        }
        set
        {

            if (!ReferenceEquals(_managedAccountPrograms, value))
            {
                var previousValue = _managedAccountPrograms as FixupCollection<ManagedAccountProgram>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupManagedAccountPrograms;
                }
                _managedAccountPrograms = value;
                var newValue = value as FixupCollection<ManagedAccountProgram>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupManagedAccountPrograms;
                }
            }

        }
    }
    private ICollection<ManagedAccountProgram> _managedAccountPrograms;



    public virtual ICollection<ClientNote> ClientNotes
    {
        get
        {
            if (_clientNotes == null)
            {

                var newCollection = new FixupCollection<ClientNote>();
                newCollection.CollectionChanged += FixupClientNotes;
                _clientNotes = newCollection;

            }
            return _clientNotes;
        }
        set
        {

            if (!ReferenceEquals(_clientNotes, value))
            {
                var previousValue = _clientNotes as FixupCollection<ClientNote>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupClientNotes;
                }
                _clientNotes = value;
                var newValue = value as FixupCollection<ClientNote>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupClientNotes;
                }
            }

        }
    }
    private ICollection<ClientNote> _clientNotes;



    public virtual ICollection<InternalUserMessage> InternalUserMessages
    {
        get
        {
            if (_internalUserMessages == null)
            {

                var newCollection = new FixupCollection<InternalUserMessage>();
                newCollection.CollectionChanged += FixupInternalUserMessages;
                _internalUserMessages = newCollection;

            }
            return _internalUserMessages;
        }
        set
        {

            if (!ReferenceEquals(_internalUserMessages, value))
            {
                var previousValue = _internalUserMessages as FixupCollection<InternalUserMessage>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupInternalUserMessages;
                }
                _internalUserMessages = value;
                var newValue = value as FixupCollection<InternalUserMessage>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupInternalUserMessages;
                }
            }

        }
    }
    private ICollection<InternalUserMessage> _internalUserMessages;



    public virtual ICollection<InternalUserMessage> InternalUserMessages1
    {
        get
        {
            if (_internalUserMessages1 == null)
            {

                var newCollection = new FixupCollection<InternalUserMessage>();
                newCollection.CollectionChanged += FixupInternalUserMessages1;
                _internalUserMessages1 = newCollection;

            }
            return _internalUserMessages1;
        }
        set
        {

            if (!ReferenceEquals(_internalUserMessages1, value))
            {
                var previousValue = _internalUserMessages1 as FixupCollection<InternalUserMessage>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupInternalUserMessages1;
                }
                _internalUserMessages1 = value;
                var newValue = value as FixupCollection<InternalUserMessage>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupInternalUserMessages1;
                }
            }

        }
    }
    private ICollection<InternalUserMessage> _internalUserMessages1;



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



    public virtual ICollection<TransferActivity> TransferActivities
    {
        get
        {
            if (_transferActivities == null)
            {

                var newCollection = new FixupCollection<TransferActivity>();
                newCollection.CollectionChanged += FixupTransferActivities;
                _transferActivities = newCollection;

            }
            return _transferActivities;
        }
        set
        {

            if (!ReferenceEquals(_transferActivities, value))
            {
                var previousValue = _transferActivities as FixupCollection<TransferActivity>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupTransferActivities;
                }
                _transferActivities = value;
                var newValue = value as FixupCollection<TransferActivity>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupTransferActivities;
                }
            }

        }
    }
    private ICollection<TransferActivity> _transferActivities;



    public virtual ICollection<TransferActivity> TransferActivities1
    {
        get
        {
            if (_transferActivities1 == null)
            {

                var newCollection = new FixupCollection<TransferActivity>();
                newCollection.CollectionChanged += FixupTransferActivities1;
                _transferActivities1 = newCollection;

            }
            return _transferActivities1;
        }
        set
        {

            if (!ReferenceEquals(_transferActivities1, value))
            {
                var previousValue = _transferActivities1 as FixupCollection<TransferActivity>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupTransferActivities1;
                }
                _transferActivities1 = value;
                var newValue = value as FixupCollection<TransferActivity>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupTransferActivities1;
                }
            }

        }
    }
    private ICollection<TransferActivity> _transferActivities1;



    public virtual ICollection<ConversionActivity> ConversionActivities
    {
        get
        {
            if (_conversionActivities == null)
            {

                var newCollection = new FixupCollection<ConversionActivity>();
                newCollection.CollectionChanged += FixupConversionActivities;
                _conversionActivities = newCollection;

            }
            return _conversionActivities;
        }
        set
        {

            if (!ReferenceEquals(_conversionActivities, value))
            {
                var previousValue = _conversionActivities as FixupCollection<ConversionActivity>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupConversionActivities;
                }
                _conversionActivities = value;
                var newValue = value as FixupCollection<ConversionActivity>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupConversionActivities;
                }
            }

        }
    }
    private ICollection<ConversionActivity> _conversionActivities;



    public virtual ICollection<ConversionActivity> ConversionActivities1
    {
        get
        {
            if (_conversionActivities1 == null)
            {

                var newCollection = new FixupCollection<ConversionActivity>();
                newCollection.CollectionChanged += FixupConversionActivities1;
                _conversionActivities1 = newCollection;

            }
            return _conversionActivities1;
        }
        set
        {

            if (!ReferenceEquals(_conversionActivities1, value))
            {
                var previousValue = _conversionActivities1 as FixupCollection<ConversionActivity>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupConversionActivities1;
                }
                _conversionActivities1 = value;
                var newValue = value as FixupCollection<ConversionActivity>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupConversionActivities1;
                }
            }

        }
    }
    private ICollection<ConversionActivity> _conversionActivities1;



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



    public virtual ICollection<AdminTransaction> AdminTransactions
    {
        get
        {
            if (_adminTransactions == null)
            {

                var newCollection = new FixupCollection<AdminTransaction>();
                newCollection.CollectionChanged += FixupAdminTransactions;
                _adminTransactions = newCollection;

            }
            return _adminTransactions;
        }
        set
        {

            if (!ReferenceEquals(_adminTransactions, value))
            {
                var previousValue = _adminTransactions as FixupCollection<AdminTransaction>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupAdminTransactions;
                }
                _adminTransactions = value;
                var newValue = value as FixupCollection<AdminTransaction>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupAdminTransactions;
                }
            }

        }
    }
    private ICollection<AdminTransaction> _adminTransactions;



    public virtual ICollection<AdminTransaction> AdminTransactions1
    {
        get
        {
            if (_adminTransactions1 == null)
            {

                var newCollection = new FixupCollection<AdminTransaction>();
                newCollection.CollectionChanged += FixupAdminTransactions1;
                _adminTransactions1 = newCollection;

            }
            return _adminTransactions1;
        }
        set
        {

            if (!ReferenceEquals(_adminTransactions1, value))
            {
                var previousValue = _adminTransactions1 as FixupCollection<AdminTransaction>;
                if (previousValue != null)
                {
                    previousValue.CollectionChanged -= FixupAdminTransactions1;
                }
                _adminTransactions1 = value;
                var newValue = value as FixupCollection<AdminTransaction>;
                if (newValue != null)
                {
                    newValue.CollectionChanged += FixupAdminTransactions1;
                }
            }

        }
    }
    private ICollection<AdminTransaction> _adminTransactions1;

        #endregion

        #region Association Fixup
    

    private bool _settingFK = false;


    private void FixupL_AccountFormType(L_AccountFormType previousValue)
    {

        if (previousValue != null && previousValue.Users.Contains(this))
        {
            previousValue.Users.Remove(this);
        }


        if (L_AccountFormType != null)
        {
            if (!L_AccountFormType.Users.Contains(this))
            {
                L_AccountFormType.Users.Add(this);
            }

            if (FK_UserTypeID != L_AccountFormType.PK_AccountFormID)

            {
                FK_UserTypeID = L_AccountFormType.PK_AccountFormID;
            }

        }

        else if (!_settingFK)

        {

            FK_UserTypeID = null;

        }

    }


    private void FixupOrganization(Organization previousValue)
    {

        if (previousValue != null && previousValue.Users.Contains(this))
        {
            previousValue.Users.Remove(this);
        }


        if (Organization != null)
        {
            if (!Organization.Users.Contains(this))
            {
                Organization.Users.Add(this);
            }

            if (FK_OrganizationID != Organization.PK_OrganizationID)

            {
                FK_OrganizationID = Organization.PK_OrganizationID;
            }

        }

    }


    private void FixupAssetManagers(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (AssetManager item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (AssetManager item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupClients(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (Client item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (Client item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupIntroducingBrokers(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (IntroducingBroker item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (IntroducingBroker item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupPartnerCommissions(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (PartnerCommission item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (PartnerCommission item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupUserImages(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (UserImage item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (UserImage item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupUserDocuments(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (UserDocument item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (UserDocument item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupManagedAccountPrograms(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (ManagedAccountProgram item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (ManagedAccountProgram item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupClientNotes(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (ClientNote item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (ClientNote item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupInternalUserMessages(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (InternalUserMessage item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (InternalUserMessage item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupInternalUserMessages1(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (InternalUserMessage item in e.NewItems)
            {

                item.User1 = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (InternalUserMessage item in e.OldItems)
            {

                if (ReferenceEquals(item.User1, this))
                {
                    item.User1 = null;
                }

            }
        }
    }


    private void FixupUserActivities(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (UserActivity item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (UserActivity item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupTransferActivities(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (TransferActivity item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (TransferActivity item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupTransferActivities1(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (TransferActivity item in e.NewItems)
            {

                item.User1 = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (TransferActivity item in e.OldItems)
            {

                if (ReferenceEquals(item.User1, this))
                {
                    item.User1 = null;
                }

            }
        }
    }


    private void FixupConversionActivities(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (ConversionActivity item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (ConversionActivity item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupConversionActivities1(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (ConversionActivity item in e.NewItems)
            {

                item.User1 = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (ConversionActivity item in e.OldItems)
            {

                if (ReferenceEquals(item.User1, this))
                {
                    item.User1 = null;
                }

            }
        }
    }


    private void FixupAdminTransactions(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (AdminTransaction item in e.NewItems)
            {

                item.User = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (AdminTransaction item in e.OldItems)
            {

                if (ReferenceEquals(item.User, this))
                {
                    item.User = null;
                }

            }
        }
    }


    private void FixupAdminTransactions1(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (AdminTransaction item in e.NewItems)
            {

                item.User1 = this;

            }
        }

        if (e.OldItems != null)
        {
            foreach (AdminTransaction item in e.OldItems)
            {

                if (ReferenceEquals(item.User1, this))
                {
                    item.User1 = null;
                }

            }
        }
    }

        #endregion

    
}

}
